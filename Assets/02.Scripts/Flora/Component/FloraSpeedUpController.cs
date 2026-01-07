using System;
using UnityEngine;

[RequireComponent(typeof(FloraStats))]
public class FloraSpeedUpController : MonoBehaviour
{
    [Serializable]
    public struct SpeedTier
    {
        public float TierLevel;
        public float Multiplier;

        public SpeedTier(float tierLevel, float multiplier)
        {
            TierLevel = tierLevel;
            Multiplier = multiplier;
        }
    }

    private FloraStats _stats;
    
    [Header("Gauge")]
    [SerializeField] private FloraSpeedGauge _gauge;
    
    [Header("Speed Rule")]
    [SerializeField] private SpeedTier[] _speedTiers;

    public event Action<float, float> GaugeChanged;

    private void Awake()
    {
        _stats = GetComponent<FloraStats>();
        if (_speedTiers != null)
        {
            Array.Sort(_speedTiers, (a, b) => a.TierLevel.CompareTo(b.TierLevel));
        }
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

    public void AddGauge(float amount)
    {
        _gauge.AddGauge(amount);
    }

    public void SetGauge(float value)
    {
        _gauge.Set(value);
    }

    private void OnGaugeValueChanged(float current, float max)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log($"[Flora] Gauge: {current:F2} / {max:F2}");
#endif

        GaugeChanged?.Invoke(current, max);

        EvaluateMultiplier(current);
    }

    private void EvaluateMultiplier(float gauge)
    {
        for (int i = _speedTiers.Length - 1; i >= 0; i--)
        {
            if (gauge >= _speedTiers[i].TierLevel)
            {
                float multiplier = _speedTiers[i].Multiplier;
                _stats.SetSpeedMultiplier(multiplier);
                break;
            }
        }
    }
}
