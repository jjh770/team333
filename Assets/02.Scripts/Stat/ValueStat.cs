using System;
using UnityEngine;

[Serializable]
public class ValueStat
{
    [SerializeField] private float _value;

    public float Value => _value;

    public event Action<float> OnValueChanged;

    public void Initialize()
    {
        OnValueChanged?.Invoke(_value);
    }

    public void SetValue(float amount)
    {
        _value = amount;
        OnValueChanged?.Invoke(_value);
    }

    public void Increase(float amount)
    {
        _value += amount;
        OnValueChanged?.Invoke(_value);
    }

    public void Decrease(float amount)
    {
        _value -= amount;
        OnValueChanged?.Invoke(_value);
    }
}
