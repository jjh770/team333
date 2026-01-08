using UnityEngine;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MonsterStat))]
public class Monster : MonoBehaviour, IPoolable
{
    private NavMeshAgent _agent;
    private MonsterStat _stat;

    public event Action<Monster> OnDie;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _stat = GetComponent<MonsterStat>();
    }

    private void OnEnable()
    {
        _stat.OnDeath += Die;
    }

    private void OnDisable()
    {
        _stat.OnDeath -= Die;
    }

    public void OnSpawn()
    {
        _agent.enabled = true;
    }

    public void OnDespawn()
    {
        _agent.enabled = false;
    }

    public void Die()
    {
        OnDie?.Invoke(this);
    }
}
