using UnityEngine;

public class BeezPetItem : MonoBehaviour, IInteractable
{
    public InteractionType Type => InteractionType.Use;
    public IconType IconType => IconType.Beez;
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
        if (interactor.TryGetComponent<PlayerPet>(out var pet))
        {
            pet.TryAddPet();
            Destroy(gameObject);
        }
    }
}
