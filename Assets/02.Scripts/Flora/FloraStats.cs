using System;
using UnityEngine;

public class FloraStats : MonoBehaviour
{
    [SerializeField] private DerivedStat _moveSpeed;

    public float CurrentSpeed => _moveSpeed.CurrentValue;
    public float MaxSpeed => _moveSpeed.MaxValue;
    
    public event Action<float> SpeedChanged;
    
    private void Awake()
    {
        InitializeStats();
    }

    private void InitializeStats()
    {
        _moveSpeed.Initialize();
    }
    
    private void OnEnable()
    {
        _moveSpeed.OnValueChanged += OnSpeedChanged;
    }
    
    private void OnDisable()
    {
        _moveSpeed.OnValueChanged -= OnSpeedChanged;
    }
    
    private void OnSpeedChanged(float current) => SpeedChanged?.Invoke(current);
}
