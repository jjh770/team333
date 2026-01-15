using System;
using UnityEngine;

public class QuestTree : MonoBehaviour, IDamageable
{
    [SerializeField] private ConsumableStat _health;

    public event Action OnTreeDestroyed;

    public event Action<float, float> OnHealthChanged
    {
        add => _health.OnValueChanged += value;
        remove => _health.OnValueChanged -= value;
    }

    private void Start()
    {
        _health.Initialize(_health.MaxValue);
    }

    public bool TryTakeDamage(Damage damage)
    {
        if (damage.Value <= 0) return false;
        
        _health.Decrease(damage.Value);

        if (_health.IsEmpty)
        {
            OnTreeDestroyed?.Invoke();
            Destroy(gameObject);
        }
        return true;
    }
}
