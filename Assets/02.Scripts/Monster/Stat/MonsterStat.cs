using UnityEngine;
using System;

public class MonsterStat : MonoBehaviour
{
    [SerializeField] private MonsterData _data;

    public ConsumableStat Health { get; private set; } = new();
    public ValueStat AttackDamage { get; private set; } = new();
    public ValueStat MoveSpeed { get; private set; } = new();

    public event Action<float, float> OnHealthChanged
    {
        add => Health.OnValueChanged += value;
        remove => Health.OnValueChanged -= value;
    }

    private void Start()
    {
        if (_data == null) return;

        Health.Initialize(_data.MaxHealth);
        AttackDamage.Initialize(_data.AttackDamage);
        MoveSpeed.Initialize(_data.MoveSpeed);
    }
}
