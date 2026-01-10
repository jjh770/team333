using UnityEngine;

public class SkillUnlocker : ItemBase
{
    public override InteractionType Type => InteractionType.Use;

    public override bool CanInteract => !_isHeld;

    public override void Interact(GameObject interactor)
    {
        if (interactor.TryGetComponent<PlayerSkillController>(out var controller))
        {
            if (!controller.IsUnlocked)
            {
                controller.UnlockSkill();
                Destroy(gameObject);
            }
        }
    }
}
