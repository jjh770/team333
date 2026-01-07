using UnityEngine;
using System;

public class MonsterStat : MonoBehaviour
{
    public ConsumableStat Health;
    public ValueStat AttackDamage;
    public ValueStat MoveSpeed;

    private void Start()
    {
        Health.Initialize();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Health.Decrease(10);
        }
    }
}
