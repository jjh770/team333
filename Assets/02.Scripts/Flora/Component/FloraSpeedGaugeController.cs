using System;
using UnityEngine;

[RequireComponent(typeof(FloraStats))]
public class FloraSpeedGaugeController : MonoBehaviour
{
    private FloraStats _stats;

    [Header("Gauge")]
    [SerializeField] private FloraSpeedGauge _gauge;

    [Header("Speed Rule")]
    [SerializeField] private float _minMultiplier = 1f;
    [SerializeField] private float _maxMultiplier = 2.5f;

    public event Action<float, float> GaugeChanged;

    private void Awake()
    {
        _stats = GetComponent<FloraStats>();
    }

    private void OnEnable()
    {
        _gauge.OnValueChanged += OnGaugeValueChanged;
        _gauge.Initialize();
    }

    private void OnDisable()
    {
        _gauge.OnValueChanged -= OnGaugeValueChanged;
    }

    private void Update()
    {
        _gauge.Drain(Time.deltaTime);
    }

    public bool IsFull => _gauge.IsFull; 
    public bool TryAddGauge(float amount)
    {
        if (_gauge.IsFull)
        {
            return false;
        }

        _gauge.AddGauge(amount);
        return true;
    }

    public void SetGauge(float value)
    {
        _gauge.Set(value);
    }

    public void DrainGauge(float amount)
    {
        _gauge.DrainGauge(amount);
    }

    private void OnGaugeValueChanged(float current, float max)
    {
        GaugeChanged?.Invoke(current, max);

        EvaluateMultiplier(current);
    }

    private void EvaluateMultiplier(float gauge)
    {
        float percent = gauge / _gauge.MaxValue;
        float multiplier = Mathf.Lerp(_minMultiplier, _maxMultiplier, percent);

        _stats.SetSpeedMultiplier(multiplier);
    }
}
