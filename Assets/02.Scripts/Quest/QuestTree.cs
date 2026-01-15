using System;
using UnityEngine;

public class QuestTree : MonoBehaviour, IDamageable
{
    [SerializeField] private ConsumableStat _health;

    public event Action OnTreeDestroyed;

    public bool TryTakeDamage(Damage damage)
    {
        if (damage.Value <= 0) return false;
        
        // TODO: 벌은 데미지 업

        _health.Decrease(damage.Value);
        Debug.Log(_health.CurrentValue);

        if (_health.IsEmpty)
        {
            OnTreeDestroyed?.Invoke();
            Destroy(gameObject);
        }
        return true;
    }
}
