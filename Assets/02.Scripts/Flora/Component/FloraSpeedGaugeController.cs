using System;
using UnityEngine;

[RequireComponent(typeof(FloraStats))]
public class FloraSpeedGaugeController : MonoBehaviour
{
    private FloraStats _stats;
    private FloraMovement _movement;

    [Header("Gauge")]
    [SerializeField] private FloraSpeedGauge _gauge;

    [Header("Speed Rule")]
    [SerializeField] private float _minMultiplier = 1f;
    [SerializeField] private float _maxMultiplier = 2.5f;

    private bool _isDrainLocked;
    
    public event Action<float, float> GaugeChanged;

    private void Awake()
    {
        _movement = GetComponent<FloraMovement>();
        _stats = GetComponent<FloraStats>();
    }

    private void OnEnable()
    {
        _gauge.OnValueChanged += OnGaugeValueChanged;
        _gauge.Initialize();
        _movement.OnStateChanged += OnStateChanged;
    }

    private void OnDisable()
    {
        _gauge.OnValueChanged -= OnGaugeValueChanged;
        _movement.OnStateChanged -= OnStateChanged;
    }

    private void Update()
    {
        if (_isDrainLocked)
            return;
        
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
    
    private void OnStateChanged(IFloraState state)
    {
        _isDrainLocked = state is FloraWaitState;
    }
}
