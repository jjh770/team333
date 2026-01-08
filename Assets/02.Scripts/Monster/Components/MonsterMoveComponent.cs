using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MonsterStateController))]
public abstract class MonsterMoveComponent : MonoBehaviour
{
    [SerializeField] protected float _velocityThreshold = 0.1f;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 5f;
    private const float MinLookDirectionSqrMagnitude = 1e-6f;

    protected NavMeshAgent _agent;
    protected Transform _player;

    protected IAnimationStateChanger _monsterController;


    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _monsterController = GetComponent<IAnimationStateChanger>();
    }

    protected virtual void Start()
    {
        FindTarget();
    }

    protected void FindTarget()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            _player = playerObject.transform;
        }
    }

    protected void LookAtTarget()
    {
        Vector3 direction = _player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > MinLookDirectionSqrMagnitude)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }

    public void LookAtTargetImmediate()
    {
        if (_player == null) return;

        Vector3 direction = _player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > MinLookDirectionSqrMagnitude)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
