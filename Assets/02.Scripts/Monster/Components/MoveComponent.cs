using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class MoveComponent : MonoBehaviour
{
    protected NavMeshAgent _agent;
    protected Transform _player;

    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
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
