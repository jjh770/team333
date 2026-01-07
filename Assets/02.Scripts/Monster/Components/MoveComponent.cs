using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MonsterController))]
public abstract class MoveComponent : MonoBehaviour
{
    protected NavMeshAgent _agent;
    protected MonsterController _monsterController;
    protected Transform _player;

    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _monsterController = GetComponent<MonsterController>();
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
            this._player = playerObject.transform;
        }
    }
}
