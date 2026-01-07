using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MonsterController))]
public abstract class MoveComponent : MonoBehaviour
{
    [SerializeField] protected float _velocityThreshold = 0.1f;

    protected NavMeshAgent _agent;
    protected Transform _player;

    protected IAnimationStateChanger _monsterController;

    public bool IsMoving { get; private set; }

    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _monsterController = GetComponent<IAnimationStateChanger>();
    }

    protected virtual void Start()
    {
        FindTarget();
    }

    protected virtual void Update()
    {
        UpdateMoveState();
    }

    protected void UpdateMoveState()
    {
        bool isMoving = _agent.velocity.sqrMagnitude > _velocityThreshold * _velocityThreshold;

        if (IsMoving != isMoving)
        {
            IsMoving = isMoving;
            var state = IsMoving ? MonsterState.Move : MonsterState.Idle;
            _monsterController.ChangeState(state);
        }
    }

    protected void FindTarget()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            _player = playerObject.transform;
        }
    }
}
