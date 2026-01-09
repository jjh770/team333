using UnityEngine;

public class Wood : ItemBase, IPickable
{
    [Header("Settings")]
    [SerializeField] private int _addWoodAmount = 1;

    public override bool CanThrow => true;

    public Transform Transform => transform;

    private const string FloraTag = "Flora";

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
        PickUp(holdPoint);
    }

    public void OnThrown(Vector3 direction, float force)
    {
        Drop();
        _rigidbody.AddForce(direction * force, ForceMode.Impulse);
    }
}
