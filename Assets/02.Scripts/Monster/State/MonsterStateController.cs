using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(MonsterMoveComponent))]
[RequireComponent(typeof(MonsterAttackComponent))]
[RequireComponent(typeof(MonsterDamageComponent))]
[RequireComponent(typeof(MonsterController))]
public class MonsterStateController : MonoBehaviour
{
    private Animator _animator;
    private MonsterState _currentState = MonsterState.Idle;

    private MonsterAttackComponent _attackComponent;
    private MonsterMoveComponent _moveComponent;
    private MonsterDamageComponent _damageComponent;
    private MonsterController _controller;

    public MonsterState CurrentState => _currentState;

    private static readonly int s_animationHash = Animator.StringToHash("animation");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _moveComponent = GetComponent<MonsterMoveComponent>();
        _attackComponent = GetComponent<MonsterAttackComponent>();
        _damageComponent = GetComponent<MonsterDamageComponent>();
        _controller = GetComponent<MonsterController>();
    }

    private void Update()
    {
        DetermineCurrentState();
    }

    private void DetermineCurrentState()
    {
        // 1순위: 사망
        if (_controller.IsDead)
        {
            ChangeState(MonsterState.Die);
            return;
        }
        // 2순위: 타격
        if (_damageComponent.IsDamaged)
        {
            ChangeState(MonsterState.Damage);
            return;
        }

        // 3순위: 공격
        if (_attackComponent.IsAttacking)
        {
            ChangeState(MonsterState.Attack);
            return;
        }

        // 4순위: 플레이어 추적
        if (_moveComponent.IsMoving)
        {
            ChangeState(MonsterState.Move);
        }

        // 5순위: 기본
        else
        {
            ChangeState(MonsterState.Idle);
        }
    }

    public void ChangeState(MonsterState newState)
    {
        if (_currentState == newState) return;

        _currentState = newState;
        _animator.SetInteger(s_animationHash, (int)_currentState);
    }
}
