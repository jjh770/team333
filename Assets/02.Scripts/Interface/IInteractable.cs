using UnityEngine;

public interface IInteractable
{
    InteractionType Type { get; }
    Transform Transform { get; }
    bool CanInteract { get; }
    void Interact(GameObject interactor);
}
