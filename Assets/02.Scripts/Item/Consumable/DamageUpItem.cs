using UnityEngine;

public class DamageUpItem : ConsumableItemBase
{
    [SerializeField] private float _damageUpAmount;

    public override IconType IconType => IconType.Potion;

    protected override bool OnConsume(GameObject interactor)
    {
        // TODO: 공격력 올려주기
        return true;
    }
}
