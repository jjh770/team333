using UnityEngine;
using System;

public class MonsterStat : MonoBehaviour
{
    [SerializeField] private MonsterDataSO _data;
    public MonsterDataSO Data => _data;

    public ConsumableStat Health { get; private set; } = new();
    public ValueStat AttackDamage { get; private set; } = new();
    public ValueStat AttackCooltime { get; private set; } = new();
    public ValueStat MoveSpeed { get; private set; } = new();

    public float GetAttackCooltime() => AttackCooltime.Value;
    public float GetMoveSpeed() => MoveSpeed.Value;
    public float GetAttackDamage() => AttackDamage.Value;

    public event Action<float, float> OnHealthChanged
    {
        add => Health.OnValueChanged += value;
        remove => Health.OnValueChanged -= value;
    }

    public event Action<int> OnDamaged;

    private void OnEnable()
    {
        if (_data == null)
        {
            Debug.LogError($"MonsterData가 {gameObject.name}에 할당되지 않았습니다.", gameObject);
            return;
        }

        Health.Initialize(_data.MaxHealth);
        AttackDamage.Initialize(_data.AttackDamage);
        AttackCooltime.Initialize(_data.AttackCooltime);
        MoveSpeed.Initialize(_data.MoveSpeed);
    }

    public void SetMoveSpeed(float value)
    {
        MoveSpeed.SetValue(value);
    }

    public void ChangeMoveSpeed(float amount)
    {
        if(amount >= 0)
        {
            MoveSpeed.Increase(amount);
        }
        else
        {
            MoveSpeed.Decrease(-amount);
        }
    }
    public void TakeDamage(float amount)
    {
        Health.Decrease(amount);
        OnDamaged?.Invoke((int)amount);
    }

}
