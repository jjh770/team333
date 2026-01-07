using System;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Moving,
    Attacking,
    Dashing
}

public class PlayerStateManager : MonoBehaviour
{
    [SerializeField] private PlayerState _currentState = PlayerState.Idle;

    public PlayerState CurrentState => _currentState;

    public event Action<PlayerState, PlayerState> OnStateChanged;

    public bool CanMove => _currentState == PlayerState.Idle || _currentState == PlayerState.Moving || _currentState == PlayerState.Attacking;
    public bool CanAttack => _currentState == PlayerState.Idle || _currentState == PlayerState.Moving || _currentState == PlayerState.Attacking;
    public bool CanDash => _currentState != PlayerState.Dashing;

    public void ChangeState(PlayerState newState)
    {
        if (_currentState == newState)
            return;

        PlayerState previousState = _currentState;

        // 상태 전환 규칙 검증
        if (!IsValidTransition(previousState, newState))
        {
            return;
        }

        _currentState = newState;
        OnStateChanged?.Invoke(previousState, newState);
    }

    private bool IsValidTransition(PlayerState from, PlayerState to)
    {
        if (to == PlayerState.Idle)
            return true;

        switch (from)
        {
            case PlayerState.Idle:
                return true; // Idle에서는 모든 상태로 전환 가능

            case PlayerState.Moving:
                return true; // Moving에서도 모든 상태로 전환 가능

            case PlayerState.Attacking:
                // 공격 중에는 이동 또는 대시로 전환 가능
                return to == PlayerState.Moving || to == PlayerState.Dashing;

            case PlayerState.Dashing:
                // 대시 중에는 상태 전환 불가 (대시가 끝나야 함)
                return false;

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
        foreach (var state in states)
        {
            if (_currentState == state)
                return true;
        }
        return false;
    }
}
