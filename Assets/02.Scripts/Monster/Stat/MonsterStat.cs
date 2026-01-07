using UnityEngine;
using System;

public class MonsterStat : MonoBehaviour
{
    [field: SerializeField] public ConsumableStat Health { get; private set; }
    [field: SerializeField] public ValueStat AttackDamage { get; private set; }
    [field: SerializeField] public ValueStat MoveSpeed { get; private set; }

    public event Action<float, float> OnHealthChanged
    {
        add => Health.OnValueChanged += value;
        remove => Health.OnValueChanged -= value;
    }

    private void Start()
    {
        Health?.Initialize();
        AttackDamage?.Initialize();
        MoveSpeed?.Initialize();
    }
}
