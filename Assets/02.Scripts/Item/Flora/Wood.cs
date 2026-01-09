using UnityEngine;

public class Wood : ItemBase, IPickable
{
    [Header("Settings")]
    [SerializeField] private int _addWoodAmount = 1;

    public override bool CanThrow => true;

    private const string FloraTag = "Flora";

    // IInteractable 구현 (IPickable이 상속)
    public Transform Transform => transform;
    public InteractionType Type => InteractionType.PickUp;
    public bool CanInteract => !_isHeld; // 들고 있지 않을 때만 상호작용 가능

    private void OnCollisionEnter(Collision other)
    {
        if (_isHeld) return;

        if (!other.gameObject.CompareTag(FloraTag)) return;
        if (!other.gameObject.TryGetComponent<FloraInteraction>(out var interaction)) return;

        interaction.AddWood(_addWoodAmount);
        Destroy(gameObject);
    }

    public void OnPickedUp(Transform holdPoint)
    {
        if (holdPoint == null) return;
        PickUp(holdPoint);
    }

    public void OnThrown(Vector3 direction, float force)
    {
        Drop();
        if (_rigidbody != null)
        {
            _rigidbody.AddForce(direction * force, ForceMode.Impulse);
        }
    }

    public void Interact(GameObject interactor)
    {
        // PlayerInteraction이 E키 눌렀을 때 호출
        // 실제 줍기는 PlayerPickUpThrow의 HandleInteract에서 처리
        // 이 메서드는 호출되지만 실제 로직은 OnPickedUp에서 실행됨
    }
    private void OnDestroy()
    {
        InteractionEvents.NotifyDestroyed(this);
    }
}
