using System;
using UnityEngine;

public class FloraInventory : MonoBehaviour
{
    [SerializeField] private ValueStat _wood;

    public float WoodCount => _wood.Value;

    public event Action<float> OnWoodChanged;

    private void OnEnable()
    {
        _wood.OnValueChanged += HandleWoodChanged;
    }

    private void OnDisable()
    {
        _wood.OnValueChanged -= HandleWoodChanged;
    }

    public void AddWood(int amount)
    {
        _wood.Increase(amount);
    }

    public bool TryUseWood(int amount)
    {
        if (_wood.Value < amount) return false;

        _wood.Decrease(amount);
        return true;
    }

    private void HandleWoodChanged(float value)
    {
        OnWoodChanged?.Invoke(value);
    }
}