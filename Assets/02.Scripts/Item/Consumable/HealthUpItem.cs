using UnityEngine;

public class HealthUpItem : MonoBehaviour, IInteractable
{
    [SerializeField] private float _healthUpAmount;
    private ItemFactory _itemFactory;

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
        // 체력 올려주기
        if (interactor.TryGetComponent(out IHealable healable))
        {
            healable.IncreaseHealth(_healthUpAmount);
            _itemFactory.Despawn(this.gameObject);
        }
    }
}
