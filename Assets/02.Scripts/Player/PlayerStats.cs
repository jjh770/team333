using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private ConsumableStat _health;
    [SerializeField] private ConsumableStat _speed;
    [SerializeField] private ValueStat _jumpForce;

    public ConsumableStat Health => _health;
    public ConsumableStat Speed => _speed;
    public ValueStat JumpForce => _jumpForce;

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

    public bool IsDead => _health.IsEmpty;
}
