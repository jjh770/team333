using System;
using UnityEngine;

[Serializable]
public class DerivedStat
{
    [SerializeField] private float _minValue;
    [SerializeField] private float _maxValue;

    [SerializeField] private float _baseValue;
    [SerializeField] private float _additive;
    [SerializeField] private float _multiplier = 1f;

    private float _cachedValue;

    public float MinValue => _minValue;
    public float MaxValue => _maxValue;

    public float CurrentValue
    {
        get
        {
            float value =
                (_baseValue + _additive) * _multiplier;

            return Mathf.Clamp(value, _minValue, _maxValue);
        }
    }

    public event Action<float> OnValueChanged;

    public void Initialize()
    {
        _cachedValue = CurrentValue;
        OnValueChanged?.Invoke(_cachedValue);
    }


    public void SetBaseValue(float value)
    {
        _baseValue = value;
        NotifyIfChanged();
    }

    public void AddModifier(float value)
    {
        _additive += value;
        NotifyIfChanged();
    }

    public void SetMultiplier(float value)
    {
        _multiplier = Mathf.Max(0f, value);
        NotifyIfChanged();
    }

    public void SetMaxValue(float value)
    {
        _maxValue = Mathf.Max(_minValue, value);
        NotifyIfChanged();
    }

    private void NotifyIfChanged()
    {
        float newValue = CurrentValue;

        if (!Mathf.Approximately(newValue, _cachedValue))
        {
            _cachedValue = newValue;
            OnValueChanged?.Invoke(_cachedValue);
        }
    }
}