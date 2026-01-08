using System;
using UnityEngine;

public class MonsterDamageComponent : MonoBehaviour, IDamageable
{
    private MonsterStat _monsterStat;

    private void Awake()
    {
        _monsterStat = GetComponent<MonsterStat>();
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
        return true;
    }
}
