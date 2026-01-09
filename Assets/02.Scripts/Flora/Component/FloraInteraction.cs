using UnityEngine;

[RequireComponent(typeof(FloraInventory))]
[RequireComponent(typeof(FloraSpeedGaugeController))]
public class FloraInteraction : MonoBehaviour, ITalkable
{
    [Header("Wood Interaction Settings")]
    [SerializeField] private int _woodCost = 1;
    [SerializeField] private float _gaugeAmount = 0.2f;

    private FloraInventory _inventory;
    private FloraSpeedGaugeController _gaugeController;
    private FloraMovement _movement;
    private FloraSkillController _skillController;

    public InteractionType Type => InteractionType.Talk;

    public Transform Transform => transform;

    public bool CanInteract => true; // Todo : ÄðÅ¸ÀÓ? Á¶°Ç?

    private void Awake()
    {
        _inventory = GetComponent<FloraInventory>();
        _gaugeController = GetComponent<FloraSpeedGaugeController>();
        _movement = GetComponent<FloraMovement>();
        _skillController = GetComponent<FloraSkillController>();
    }

    public void AddWood(int woodAmount)
    {
        _inventory.AddWood(woodAmount);
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
        return _movement.Resume();
    }

    public void SetSkill(FloraSkillBase skill)
    {
        _skillController.SetSkill(skill);
    }

    public void Interact(GameObject interactor)
    {
        Debug.Log($"{interactor.name}");
        TryFeedWood();
    }
}