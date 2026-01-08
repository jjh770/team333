using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MonsterStat))]
public class MonsterController : MonoBehaviour, IPoolable
{
    [SerializeField] private float _deathAnimationDuration = 0.19f;

    private NavMeshAgent _agent;
    private MonsterStat _stat;

    public event Action<MonsterController> OnDie;

    private bool _isDead;
    public bool IsDead => _isDead;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _stat = GetComponent<MonsterStat>();
    }

    private void OnEnable()
    {
        _isDead = false;
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
        if (_isDead) return;

        _isDead = true;
        _agent.isStopped = true;

        StartCoroutine(DeathCoroutine());
    }

    private IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(_deathAnimationDuration);
        OnDie?.Invoke(this);
    }

    //public bool TryTakeDamage(Damage damage)
    //{
    //    if (damage.Value <= 0)
    //        return false;

    //    _monsterStat.DecreaseHealth(damage.Value);
    //    _monsterDetectComponent.WhiteFlash();

    //    if(_monsterStat.IsDie)
    //    {
    //        죽음 코루틴 실행
    //    }


    //    _fsm.Change
    //    return true;
    //}
}
