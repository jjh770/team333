using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private ConsumableStat _health;
    [SerializeField] private ValueStat _speed;
    [SerializeField] private ValueStat _jumpForce;

    public ConsumableStat Health => _health;
    public ValueStat Speed => _speed;
    public ValueStat JumpForce => _jumpForce;

    private void Start()
    {
        InitializeStats();
    }

    private void InitializeStats()
    {
        _health.Initialize();
    }

    public bool IsDead => _health.IsEmpty;
}
