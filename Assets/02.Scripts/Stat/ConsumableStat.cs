using System;
using UnityEngine;

[Serializable]
public class ConsumableStat
{
    [SerializeField] private float _maxValue;
    [SerializeField] private float _reganValue;
    [SerializeField] private float _reganSpeed;

    private float _currentValue;

    public float MaxValue => _maxValue;
    public float CurrentValue => _currentValue;
    public float RegenValue => _reganValue;
    public bool IsEmpty => _currentValue <= 0;
    public bool IsFull => _currentValue >= _maxValue;

    public event Action<float, float> OnValueChanged;

    public void Initialize()
    {
        _currentValue = _maxValue;
        OnValueChanged?.Invoke(_currentValue, _maxValue);
    }

    public void SetMaxValue(float amount)
    {
        _maxValue = Mathf.Max(0, amount);
        _currentValue = Mathf.Min(_currentValue, _maxValue);
        OnValueChanged?.Invoke(_currentValue, _maxValue);
    }

    public void IncreaseMax(float amount)
    {
        _maxValue += amount;
        OnValueChanged?.Invoke(_currentValue, _maxValue);
    }

    public void SetValue(float amount)
    {
        _currentValue = Mathf.Clamp(amount, 0, _maxValue);
        OnValueChanged?.Invoke(_currentValue, _maxValue);
    }

    public void Increase(float amount)
    {
        _currentValue = Mathf.Min(_currentValue + amount, _maxValue);
        OnValueChanged?.Invoke(_currentValue, _maxValue);
    }

    public void Decrease(float amount)
    {
        _currentValue = Mathf.Max(_currentValue - amount, 0);
        OnValueChanged?.Invoke(_currentValue, _maxValue);
    }

    public void Fill()
    {
        _currentValue = _maxValue;
        OnValueChanged?.Invoke(_currentValue, _maxValue);
    }

    public void Empty()
    {
        _currentValue = 0;
        OnValueChanged?.Invoke(_currentValue, _maxValue);
    }

    public void Regen()
    {
        if (_reganValue > 0 && !IsFull)
        {
            Increase(_reganValue * Time.deltaTime * _reganSpeed);
        }
    }
}
