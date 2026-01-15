using System;
using UnityEngine;

public class PlayerStat : MonoBehaviour, IHealable
{
    [SerializeField] private PlayerData _data;
    private PlayerEffectController _effectController;
    public ConsumableStat Health { get; private set; } = new();

    public event Action<float, float> OnHealthChanged
    {
        add => Health.OnValueChanged += value;
        remove => Health.OnValueChanged -= value;
    }

    public event Action OnDeath;

    private void Awake()
    {
        _effectController = GetComponent<PlayerEffectController>();
    }
    private void OnEnable()
    {
        if (_data == null)
        {
            return;
        }

        Health.Initialize(_data.Stats.MaxHealth);
        StopHealthEffect();
    }

    public void IncreaseHealth(float amount)
    {
        Health.Increase(amount);
        PlayHealthEffect();
    }

    public void DecreaseHealth(float amount)
    {
        Health.Decrease(amount);

        if (Health.IsEmpty)
        {
            OnDeath?.Invoke();
        }
    }

    private void StopHealthEffect()
    {
        _effectController?.StopEffect(PlayerEffectType.Heal);
    }

    private void PlayHealthEffect()
    {
        _effectController?.PlayEffect(PlayerEffectType.Heal);
    }
}
