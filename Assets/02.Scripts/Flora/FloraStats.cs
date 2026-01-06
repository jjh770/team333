using System;
using UnityEngine;

public class FloraStats : MonoBehaviour
{
    [SerializeField] private ConsumableStat _moveSpeed;

    public float CurrentSpeed => _moveSpeed.CurrentValue;
    public float MaxSpeed => _moveSpeed.MaxValue;
    
    public event Action<float, float> SpeedChanged;
    
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
    
    private void OnSpeedChanged(float current, float max) => SpeedChanged?.Invoke(current, max);
}
