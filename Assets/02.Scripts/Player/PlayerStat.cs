using System;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    [SerializeField] private PlayerData _data;

    public ConsumableStat Health { get; private set; } = new();

    public event Action<float, float> OnHealthChanged
    {
        add => Health.OnValueChanged += value;
        remove => Health.OnValueChanged -= value;
    }

    public event Action OnDeath;

    private void OnEnable()
    {
        if (_data == null)
        {
            return;
        }

        Health.Initialize(_data.Stats.MaxHealth);
    }

    public void DecreaseHealth(float amount)
    {
        Health.Decrease(amount);

        if (Health.IsEmpty)
        {
            OnDeath?.Invoke();
        }
    }
}
