using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class MonsterController : MonoBehaviour
{
    [SerializeField] private float _velocityThreshold = 0.1f;

    private Animator _animator;
    private NavMeshAgent _agent;
    private MonsterState _currentState = MonsterState.Idle;

    private static readonly int AnimationHash = Animator.StringToHash("animation");

    public enum MonsterState
    {
        Idle = 1,
        Move = 2,
        Attack = 3,
        Damage = 4,
        Die = 5
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        UpdateMoveState();
    }

    private void UpdateMoveState()
    {
        bool isMoving = _agent.velocity.sqrMagnitude > _velocityThreshold * _velocityThreshold;
        MonsterState newState = isMoving ? MonsterState.Move : MonsterState.Idle;

        ChangeState(newState);
    }

    public void ChangeState(MonsterState newState)
    {
        if (_currentState == newState) return;

        _currentState = newState;
        _animator.SetInteger(AnimationHash, (int)_currentState);
    }
}
