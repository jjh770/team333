using System;
using UnityEngine;

public class QuestTree : MonoBehaviour, IDamageable
{
    [SerializeField] private ConsumableStat _health;
    [SerializeField] private Transform _center;
    [SerializeField] private GameObject _endingSequence;
    [SerializeField] private FloraSound _floraSound;

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
        MonsterEffectPool.Instance.PlayHitEffect(_center.position);
        MonsterEffectPool.Instance.PlaySmokeEffect(_center.position);

        if (_health.IsEmpty)
        {
            OnTreeDestroyed?.Invoke();
            _floraSound?.PlayQuestComplete();
            if (_endingSequence != null)
            {
                _endingSequence.SetActive(true);
            }
            Destroy(gameObject);
        }
        return true;
    }
}
