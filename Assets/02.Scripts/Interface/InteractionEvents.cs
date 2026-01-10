using System;

public static class InteractionEvents
{
    public static event Action<IInteractable> OnInteractableDestroyed;

    public static void NotifyDestroyed(IInteractable interactable)
    {
        OnInteractableDestroyed?.Invoke(interactable);
    }
}