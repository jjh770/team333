using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(MonsterMoveComponent))]
public abstract class BaseMonsterController : MonoBehaviour, IPoolable
{
    protected Animator _animator;
    
    protected MonsterMoveComponent _move;

    protected MonsterState _currentState = MonsterState.Idle;
    public MonsterState CurrentState => _currentState;

    protected static readonly int s_animationHash = Animator.StringToHash("animation");

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _move = GetComponent<MonsterMoveComponent>();
    }

    abstract protected void ResetState();
    abstract protected void StopAllRoutines();
    abstract protected void FindTarget();
    abstract public void OnSpawn();
    abstract public void OnDespawn();
    abstract protected void UpdateState();
    abstract protected void ApplyState(MonsterState newState);
    abstract protected MonsterState GetCurrentState();
}
