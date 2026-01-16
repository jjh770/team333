using UnityEngine;

public class DamageUpItem : MonoBehaviour, IInteractable
{
    [SerializeField] private float _DamageUpAmount;

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
        // 공격력 올려주기
    }
}
