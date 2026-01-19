using UnityEngine;

public abstract class ConsumableItemBase : MonoBehaviour, IInteractable
{
    public InteractionType Type => InteractionType.Use;
    public abstract IconType IconType { get; }

    public Transform Transform
    {
        get
        {
            if (this == null) return null;
            return transform;
        }
    }

    public virtual bool CanInteract => true;

    public void Interact(GameObject interactor)
    {
        if (OnConsume(interactor))
        {
            if (interactor.TryGetComponent<PlayerSound>(out var sound))
            {
                sound.PlayItemConsume();
            }

            Destroy(gameObject);
        }
    }

    protected abstract bool OnConsume(GameObject interactor);
}
