using UnityEngine;

[RequireComponent(typeof(FloraInventory))]
[RequireComponent(typeof(FloraSpeedGaugeController))]
public class FloraInteraction : MonoBehaviour
{
    [Header("Wood Interaction Settings")]
    [SerializeField] private int _woodCost = 1;
    [SerializeField] private float _gaugeAmount = 0.2f;

    private FloraInventory _inventory;
    private FloraSpeedGaugeController _gaugeController;
    private FloraMovement _floraMovement;

    private void Awake()
    {
        _inventory = GetComponent<FloraInventory>();
        _gaugeController = GetComponent<FloraSpeedGaugeController>();
        _floraMovement = GetComponent<FloraMovement>();
    }

    public bool TryFeedWood()
    {
        if (_gaugeController.IsFull)
        {
            return false;
        }
        if (!_inventory.TryUseWood(_woodCost))
        {
            return false;
        }

        _gaugeController.TryAddGauge(_gaugeAmount);
        
        return true;
    }

    public bool TryResume()
    {
        return _floraMovement.Resume();
    }
}