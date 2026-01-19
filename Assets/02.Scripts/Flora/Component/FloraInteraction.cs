using UnityEngine;

[RequireComponent(typeof(FloraInventory))]
[RequireComponent(typeof(FloraSpeedGaugeController))]
public class FloraInteraction : MonoBehaviour, IInteractable
{
    [Header("Wood Interaction Settings")]
    [SerializeField] private int _woodCost = 1;
    [SerializeField] private float _gaugeAmount = 0.2f;

    private FloraInventory _inventory;
    private FloraSpeedGaugeController _gaugeController;
    private FloraMovement _movement;
    private FloraSkillController _skillController;
    private FloraSound _floraSound;
    private IFloraPath _floraPath;
    public IFloraPath FloraPath => _floraPath;

    private bool _isMoveLocked;

    private bool CanMove => _movement != null && _movement.IsWaiting && !_isMoveLocked;
    private bool CanSpeedUp => _inventory != null && _inventory.WoodCount >= _woodCost && !_gaugeController.IsFull;

    public InteractionType Type
    {
        get
        {
            if (CanMove) return InteractionType.TalkToMove;
            if (CanSpeedUp) return InteractionType.TalkToSpeedUp;
            return InteractionType.TalkToMove;
        }
    }
    public IconType IconType
    {
        get
        {
            if (CanMove) return IconType.TalkToMoveFlora;
            if (CanSpeedUp) return IconType.TalkToSpeedUpFlora;
            return IconType.TalkToMoveFlora;
        }
    }

    public Transform Transform => transform;

    public bool CanInteract => CanMove || CanSpeedUp;

    private void Awake()
    {
        _inventory = GetComponent<FloraInventory>();
        _gaugeController = GetComponent<FloraSpeedGaugeController>();
        _movement = GetComponent<FloraMovement>();
        _skillController = GetComponent<FloraSkillController>();
        _floraSound = GetComponent<FloraSound>();
        _floraPath = GetComponent<IFloraPath>();
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
        _floraSound?.PlayInteraction();

        return true;
    }

    public void AddBoard(int boardAmount)
    {
        _inventory.AddBoard(boardAmount);
    }

    public bool TryResume()
    {
        if (_movement.Resume())
        {
            _floraSound?.PlayInteraction();
            return true;
        }
        return false;
    }

    public void SetSkill(FloraSkillBase skill)
    {
        _skillController.SetSkill(skill);
    }

    public void SetMoveLock(bool isLocked)
    {
        _isMoveLocked = isLocked;
    }

    public void Interact(GameObject interactor)
    {
        if (CanMove)
        {
            TryResume();
        }
        else if (CanSpeedUp)
        {
            TryFeedWood();
        }
    }
}