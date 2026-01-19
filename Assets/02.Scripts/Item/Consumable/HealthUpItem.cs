using UnityEngine;

public class HealthUpItem : ConsumableItemBase
{
    [SerializeField] private float _healthUpAmount;

    public override IconType IconType => IconType.Potion;

    protected override bool OnConsume(GameObject interactor)
    {
        if (interactor.TryGetComponent(out IHealable healable))
        {
            healable.IncreaseHealth(_healthUpAmount);
            return true;
        }
        return false;
    }
}
