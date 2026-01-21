using UnityEngine;

public class BeezPetItem : ConsumableItemBase
{
    [SerializeField] private float _viewportMargin = 0.05f;
    private Camera _mainCamera;
    public override IconType IconType => IconType.Beez;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        EnforceCameraBounds();
    }

    protected override bool OnConsume(GameObject interactor)
    {
        if (interactor.TryGetComponent<PlayerPet>(out var pet))
        {
            pet.TryAddPet();
            return true;
        }
        return false;
    }

    private void EnforceCameraBounds()
    {
        CameraBoundsHelper.ClampPositionToCameraBounds(transform, _mainCamera, _viewportMargin);
    }
}
