using UnityEngine;

public class SkillUnlocker : ConsumableItemBase
{
    public override IconType IconType => IconType.Geezer;

    protected override bool OnConsume(GameObject interactor)
    {
        if (interactor.TryGetComponent<PlayerSkillController>(out var controller))
        {
            if (controller.SkillLevel < controller.MaxSkillLevel)
            {
                controller.UpgradeSkill();

                if (TryGetComponent<GoodSansam>(out var sansam))
                {
                    sansam.PlayEatSound();
                }

                return true;
            }
        }
        return false;
    }
}
