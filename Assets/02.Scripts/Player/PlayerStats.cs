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

    private void Update()
    {
        _speed.Regen();
    }
}
