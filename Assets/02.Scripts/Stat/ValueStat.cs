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

    public void Initialize(float value)
    {
        _value = value;
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
        _value = Mathf.Max(0, _value);
        OnValueChanged?.Invoke(_value);
    }
}
