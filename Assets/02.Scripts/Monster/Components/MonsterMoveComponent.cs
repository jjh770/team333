using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MonsterStateController))]
public abstract class MonsterMoveComponent : MonoBehaviour
{
    [SerializeField] protected float _velocityThreshold = 0.1f;

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
}
