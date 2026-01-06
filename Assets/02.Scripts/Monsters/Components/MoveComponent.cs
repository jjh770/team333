using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class MoveComponent : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Transform player;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start()
    {
        FindTarget();
    }

    protected void FindTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            this.player = player.transform;
        }
    }
}
