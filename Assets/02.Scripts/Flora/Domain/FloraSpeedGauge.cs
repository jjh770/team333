using System;
using UnityEngine;

[Serializable]
public class FloraSpeedGauge
{
    [SerializeField] private float _maxValue = 1f;
    [SerializeField] private float _drainRate;  // 초당 감소량
    private float _currentValue;
    
    public float CurrentValue => _currentValue;
    public float MaxValue => _maxValue;
    public float DrainRate => _drainRate;
    public bool IsEmpty => _currentValue <= Mathf.Epsilon;
    public bool IsFull => _currentValue >= _maxValue - Mathf.Epsilon;
    
    public event Action<float, float> OnValueChanged;
    
    public void Initialize()
    {
        _currentValue = 0f;
        OnValueChanged?.Invoke(_currentValue, _maxValue);
    }

    public void AddGauge(float amount)
    {
        Set(_currentValue + amount);
    }

    public void DrainGauge(float amount)
    {
        Set(_currentValue - amount);
    }
    
    public void Set(float value)
    {
        float prev = _currentValue;
        _currentValue = Mathf.Clamp(value, 0f, _maxValue);

        if (!Mathf.Approximately(prev, _currentValue))
            OnValueChanged?.Invoke(_currentValue, _maxValue);
    }

    public void Drain()
    {
        if (_drainRate > 0 && !IsEmpty)
        {
            DrainGauge(_drainRate * Time.deltaTime);
        }
    }
}
