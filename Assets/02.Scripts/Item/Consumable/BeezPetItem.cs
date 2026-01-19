using UnityEngine;

public class BeezPetItem : ConsumableItemBase
{
    public override IconType IconType => IconType.Beez;

    protected override bool OnConsume(GameObject interactor)
    {
        if (interactor.TryGetComponent<PlayerPet>(out var pet))
        {
            pet.TryAddPet();
            return true;
        }
        return false;
    }
}
