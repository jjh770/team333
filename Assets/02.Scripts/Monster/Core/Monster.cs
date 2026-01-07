using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour, IPoolable
{
    private MonsterStat _stat;
    private NavMeshAgent _agent;

    private void Awake()
    {
        _stat = GetComponent<MonsterStat>();
        _agent = GetComponent<NavMeshAgent>();
    }


    public void OnSpawn()
    {
        if (_agent == null) return;
        _agent.enabled = true;
    }

    public void OnDespawn()
    {
        if (_agent == null) return;
        _agent.enabled = false;
    }

    public void Die()
    {
        PoolManager.Instance.Return(gameObject);
    }
}
