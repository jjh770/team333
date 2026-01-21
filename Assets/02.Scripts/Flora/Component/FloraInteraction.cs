using UnityEngine;

[RequireComponent(typeof(FloraInventory))]
[RequireComponent(typeof(FloraSpeedGaugeController))]
public class FloraInteraction : MonoBehaviour, IInteractable
{
    [Header("Wood Interaction Settings")]
    [SerializeField] private int _woodCost = 1;
    [SerializeField] private float _gaugeAmount = 0.2f;

    [Header("Item Collect Effect")]
    [SerializeField] private ParticleSystem _collectEffectPrefab;

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

    public void AddWood(int woodAmount, Vector3 itemPosition)
    {
        _inventory.AddWood(woodAmount);
        _floraSound?.PlayGetItem();
        PlayCollectEffect(itemPosition);
    }

    private void PlayCollectEffect(Vector3 position)
    {
        if (_collectEffectPrefab == null) return;
        Instantiate(_collectEffectPrefab, position, Quaternion.identity);
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

    public void AddBoard(int boardAmount, Vector3 itemPosition)
    {
        _inventory.AddBoard(boardAmount);
        _floraSound?.PlayGetItem();
        PlayCollectEffect(itemPosition);
    }

    public bool TryResume()
    {
        bool success = _movement.Resume();
        if (success)
        {
            _floraSound?.PlayInteraction();
        }
        return success;
    }

    public void SetSkill(FloraSkillBase skill)
    {
        _skillController.SetSkill(skill);
    }

    public void SetMoveLock(bool isLocked)
    {
        _isMoveLocked = isLocked;
    }

    public void ResetForTutorialEnd()
    {
        _inventory.ResetWood();
        _gaugeController.SetGauge(0);
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