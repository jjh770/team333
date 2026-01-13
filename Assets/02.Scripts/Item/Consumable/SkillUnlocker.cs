using UnityEngine;

public class SkillUnlocker : MonoBehaviour, IInteractable
{
    public InteractionType Type => InteractionType.Use;
    public Transform Transform
    {
        get
        {
            // 이미 파괴된 상태에서 접근 시 null 반환
            if (this == null) return null;
            return transform;
        }
    }
    public bool CanInteract => true;


    public void Interact(GameObject interactor)
    {
        if (interactor.TryGetComponent<PlayerSkillController>(out var controller))
        {
            if (controller.SkillLevel < controller.MaxSkillLevel)
            {
                controller.UpgradeSkill();
                Destroy(gameObject);
            }
        }
    }
}
