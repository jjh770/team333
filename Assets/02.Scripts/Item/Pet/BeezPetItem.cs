using UnityEngine;

public class BeezPetItem : MonoBehaviour, IInteractable
{
    public InteractionType Type => InteractionType.Use;
    public Transform Transform
    {
        get
        {
            if (this == null) return null;
            return transform;
        }
    }
    public bool CanInteract => true;

    public void Interact(GameObject interactor)
    {
        // 펫 생기기
    }
}
