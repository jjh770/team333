using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MonsterDamageComponent : MonoBehaviour, IDamageable
{
    [SerializeField] private float _damageDuration = 0.09f;

    private MonsterStat _monsterStat;
    protected NavMeshAgent _agent;
    private bool _isDamaged;
    public bool IsDamaged => _isDamaged;

    private void Awake()
    {
        _monsterStat = GetComponent<MonsterStat>();
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Damage damage = new Damage(10f, null);
            TryTakeDamage(damage);
        }
    }

    public bool TryTakeDamage(Damage damage)
    {
        if (damage.Value <= 0)
            return false;

        _monsterStat.DecreaseHealth(damage.Value);

        if (!_isDamaged)
        {
            StartCoroutine(Damage_Coroutine());
        }

        return true;
    }

    private IEnumerator Damage_Coroutine()
    {
        _isDamaged = true;
        yield return new WaitForSeconds(_damageDuration);
        _isDamaged = false;
    }
}
