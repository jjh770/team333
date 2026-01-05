using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private ConsumableStat _health;
    [SerializeField] private ConsumableStat _speed;
    [SerializeField] private ValueStat _jumpForce;

    public float CurrentHealth => _health.CurrentValue;
    public float MaxHealth => _health.MaxValue;

    public float CurrentSpeed => _speed.CurrentValue;
    public float MaxSpeed => _speed.MaxValue;

    public float JumpForce => _jumpForce.Value;

    public bool IsDead => _health.IsEmpty;

    public event Action<float, float> HealthChanged;
    public event Action<float, float> SpeedChanged;
    public event Action<float> JumpForceChanged;

    private void Start()
    {
        InitializeStats();
    }

    private void InitializeStats()
    {
        _health.Initialize();
        _speed.Initialize();
        _jumpForce.Initialize();
    }

    private void OnEnable()
    {
        _health.OnValueChanged += OnHealthChanged;
        _speed.OnValueChanged += OnSpeedChanged;
        _jumpForce.OnValueChanged += OnJumpForceChanged;
    }

    private void OnDisable()
    {
        _health.OnValueChanged -= OnHealthChanged;
        _speed.OnValueChanged -= OnSpeedChanged;
        _jumpForce.OnValueChanged -= OnJumpForceChanged;
    }

    private void OnHealthChanged(float current, float max) => HealthChanged?.Invoke(current, max);
    private void OnSpeedChanged(float current, float max) => SpeedChanged?.Invoke(current, max);
    private void OnJumpForceChanged(float value) => JumpForceChanged?.Invoke(value);

    private void Update()
    {
        _speed.Regen();
    }
}
