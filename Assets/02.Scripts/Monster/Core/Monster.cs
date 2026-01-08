using UnityEngine;
using UnityEngine.AI;
using System;

[RequireComponent (typeof(NavMeshAgent))]
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
