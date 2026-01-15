using UnityEngine;

public class HealthUpItem : MonoBehaviour, IInteractable
{
    [SerializeField] private float _healthUpAmount;

    public InteractionType Type => InteractionType.Use;
    public IconType IconType => IconType.Potion;

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
        // 체력 올려주기
        if (interactor.TryGetComponent(out IHealable healable))
        {
            healable.IncreaseHealth(_healthUpAmount);
            Destroy(gameObject);
        }
    }
}
