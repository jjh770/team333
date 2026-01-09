using UnityEngine;

public interface IPickable
{
    Transform Transform { get; }
    void OnPickedUp(Transform holdPoint);
    void OnThrown(Vector3 direction, float force);
}
