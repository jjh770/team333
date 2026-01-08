using UnityEngine;
using UnityEngine.AI;
using System;

public class Monster : MonoBehaviour, IPoolable
{
    private NavMeshAgent _agent;

    public event Action<Monster> OnDie;

    private void Awake()
    {
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
        OnDie?.Invoke(this);
    }
}
