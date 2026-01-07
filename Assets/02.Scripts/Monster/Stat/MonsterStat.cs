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
    private void Awake()
    {
        Health ??= new ConsumableStat();
        AttackDamage ??= new ValueStat();
        MoveSpeed ??= new ValueStat();
    }

    private void Start()
    {
        Health.Initialize();
        AttackDamage.Initialize();
        MoveSpeed.Initialize();
    }
}
