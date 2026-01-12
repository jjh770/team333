using System;
using System.Linq;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Moving,
    Attacking,
    Dashing,
    PickUp,
    Throw,
    Skill,
    Die
}

public class PlayerStateManager : MonoBehaviour
{
    [SerializeField] private PlayerState _currentState = PlayerState.Idle;
    private PlayerInputHandler _inputHandler;
    private PlayerMove _playerMove;
    public PlayerState CurrentState => _currentState;

    public event Action<PlayerState, PlayerState> OnStateChanged;
    public event Func<PlayerState, PlayerState, bool> OnValidateStateChange;
    public bool IsDead => _currentState == PlayerState.Die;
    public bool CanMove => !IsDead && (_currentState == PlayerState.Idle || _currentState == PlayerState.Moving || _currentState == PlayerState.Attacking || _currentState == PlayerState.PickUp);
    public bool CanCarry => !IsDead && (_currentState == PlayerState.Idle || _currentState == PlayerState.Moving || _currentState == PlayerState.PickUp);
    public bool CanThrow => !IsDead && _currentState == PlayerState.PickUp;
    public bool CanAttack => !IsDead && (_currentState == PlayerState.Idle || _currentState == PlayerState.Moving || _currentState == PlayerState.Attacking);
    public bool CanDash => !IsDead && (_currentState == PlayerState.Moving || _currentState == PlayerState.Attacking);
    public bool CanSkill => !IsDead && (_currentState == PlayerState.Idle || _currentState == PlayerState.Moving);
    public bool IsHolding => _currentState == PlayerState.PickUp || _currentState == PlayerState.Throw;

    private void Awake()
    {
        _inputHandler = GetComponent<PlayerInputHandler>();
        _playerMove = GetComponent<PlayerMove>();
    }

    private void Start()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnStateChanged += HandleGameStateChanged;
        }
    }
    private void OnDestroy()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnStateChanged -= HandleGameStateChanged;
        }
    }

    private void HandleGameStateChanged(GameState oldState, GameState newState)
    {
        bool isPlaying = (newState == GameState.Playing);
        _inputHandler.enabled = isPlaying;
        _playerMove.enabled = isPlaying;

    }
    public void ChangeState(PlayerState newState)
    {
        if (_currentState == newState)
            return;

        PlayerState previousState = _currentState;

        // 상태 전환 규칙 검증
        if (!IsValidTransition(previousState, newState))
        {
            Debug.Log($"{previousState}에서는 {newState}로 갈수 없음");
            return;
        }

        _currentState = newState;
        OnStateChanged?.Invoke(previousState, newState);
    }

    private bool IsValidTransition(PlayerState from, PlayerState to)
    {
        if (from == PlayerState.Die)
            return false;

        // 외부 검증자들에게 먼저 검증 요청
        if (OnValidateStateChange != null)
        {
            foreach (var validator in OnValidateStateChange.GetInvocationList())
            {
                if (!(bool)validator.DynamicInvoke(from, to))
                    return false;
            }
        }

        // Die 상태로는 어느 상태에서든 전환 가능
        if (to == PlayerState.Die)
            return true;

        // 기본 상태 전환 규칙 검증
        if (to == PlayerState.Idle)
            return true;

        switch (from)
        {
            case PlayerState.Idle:
                return true; // Idle에서는 모든 상태로 전환 가능

            case PlayerState.Moving:
                return true; // Moving에서도 모든 상태로 전환 가능

            case PlayerState.Attacking:
                // 공격 중에는 이동, 대시, 스킬로 전환 가능
                return to == PlayerState.Moving || to == PlayerState.Dashing;

            case PlayerState.Dashing:
                // 대시 중에는 상태 전환 불가 (대시가 끝나야 함)
                return false;

            case PlayerState.PickUp:
                // PickUp 상태에서는 Throw로 전환 가능
                return to == PlayerState.Throw;

            case PlayerState.Throw:
                // Throw 상태에서는 Idle이나 PickUp으로 전환 가능
                return to == PlayerState.PickUp || to == PlayerState.Idle;

            case PlayerState.Skill:
                // Skill 상태에서는 Idle로만 전환 가능 (스킬 종료 시)
                return to == PlayerState.Idle || to == PlayerState.Moving;

            default:
                return false;
        }
    }

    public bool IsState(PlayerState state)
    {
        return _currentState == state;
    }

    public bool IsInStates(params PlayerState[] states)
    {
        return states.Contains(_currentState);
    }
}
