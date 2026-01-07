using System;
using UnityEngine;

[Serializable]
public class FloraSpeedGauge
{
    [SerializeField] private float _maxValueGauge = 1f;
    [SerializeField] private float _drainRate;  // 초당 감소량
    private float _currentValueGauge;
    
    public float CurrentValue => _currentValueGauge;
    public float MaxValue => _maxValueGauge;
    public float DrainRate => _drainRate;
    public bool IsEmpty => _currentValueGauge <= Mathf.Epsilon;
    public bool IsFull => _currentValueGauge >= _maxValueGauge - Mathf.Epsilon;
    
    public event Action<float, float> OnValueChanged;
    
    public void Initialize()
    {
        _currentValueGauge = 0f;
        OnValueChanged?.Invoke(_currentValueGauge, _maxValueGauge);
    }

    public void AddGauge(float amount)
    {
        Set(_currentValueGauge + amount);
    }

    public void DrainGauge(float amount)
    {
        Set(_currentValueGauge - amount);
    }
    
    public void Set(float value)
    {
        float prev = _currentValueGauge;
        _currentValueGauge = Mathf.Clamp(value, 0f, _maxValueGauge);

        if (!Mathf.Approximately(prev, _currentValueGauge))
            OnValueChanged?.Invoke(_currentValueGauge, _maxValueGauge);
    }

    public void Drain()
    {
        if (_drainRate > 0 && !IsEmpty)
        {
            DrainGauge(_drainRate * Time.deltaTime);
        }
    }
}
