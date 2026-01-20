using System;
using UnityEngine;

public class FloraInventory : MonoBehaviour
{
    [SerializeField] private ValueStat _wood;
    [SerializeField] private ValueStat _board;

    public float WoodCount => _wood.Value;
    public float BoardCount => _board.Value;

    public event Action<float> OnWoodChanged;
    public event Action<float> OnBoardChanged;

    private void OnEnable()
    {
        _wood.OnValueChanged += HandleWoodChanged;
        _board.OnValueChanged += HandleBoardChanged;
    }

    private void OnDisable()
    {
        _wood.OnValueChanged -= HandleWoodChanged;
        _board.OnValueChanged -= HandleBoardChanged;
    }

    public void AddWood(int amount)
    {
        _wood.Increase(amount);
    }

    public void ResetWood()
    {
        _wood.SetValue(0);
    }

    public bool TryUseWood(int amount)
    {
        if (_wood.Value < amount) return false;

        _wood.Decrease(amount);
        return true;
    }

    public void AddBoard(int amount)
    {
        _board.Increase(amount);
    }

    private void HandleWoodChanged(float value)
    {
        OnWoodChanged?.Invoke(value);
    }

    private void HandleBoardChanged(float value)
    {
        OnBoardChanged?.Invoke(value);
    }
}