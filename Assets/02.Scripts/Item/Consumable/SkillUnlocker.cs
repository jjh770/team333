using UnityEngine;

public class SkillUnlocker : MonoBehaviour, IInteractable
{
    private ItemFactory _itemFactory;

    public InteractionType Type => InteractionType.Use;
    public Transform Transform
    {
        get
        {
            // 이미 파괴된 상태에서 누군가 호출할 경우 null 반환
            if (this == null) return null;
            return transform;
        }
    }
    public bool CanInteract => true;


    public void Interact(GameObject interactor)
    {
        if (interactor.TryGetComponent<PlayerSkillController>(out var controller))
        {
            if (!controller.IsUnlocked)
            {
                controller.UnlockSkill();
                _itemFactory.Despawn(this.gameObject);
            }
        }
    }
}
