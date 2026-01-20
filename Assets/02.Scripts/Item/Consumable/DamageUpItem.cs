using UnityEngine;

public class DamageUpItem : ConsumableItemBase
{
    [SerializeField] private float _damageUpAmount;

    public override IconType IconType => IconType.DamageUpPotion;

    protected override bool OnConsume(GameObject interactor)
    {
        if (interactor.TryGetComponent(out PlayerStat playerStat))
        {
            playerStat.IncreaseDamage(_damageUpAmount);
            return true;
        }
        return false;
    }
}
