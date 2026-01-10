using UnityEngine;

public interface IPickable : IInteractable
{
    void OnPickedUp(Transform holdPoint);
    void OnThrown(Vector3 direction, float force);
}
