using System.Collections.Generic;
using UnityEngine;

public class FloraMagnet : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _magnetForce = 10f;
    [SerializeField] private float _maxSpeed = 8f;

    private readonly Dictionary<Rigidbody, ItemBase> _attractingItems = new();

    private void FixedUpdate()
    {
        AttractItems();
    }

    private void OnTriggerEnter(Collider other)
    {
        TryAddItem(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryAddItem(other);
    }

    private void OnTriggerExit(Collider other)
    {
        TryRemoveItem(other);
    }

    private void TryAddItem(Collider collider)
    {
        if (!collider.TryGetComponent<ItemBase>(out var item)) return;
        if (!item.IsThrown) return;

        if (item is not IAttractableByFlora) return;

        var rigidBody = collider.attachedRigidbody;
        if (rigidBody == null) return;

        if (_attractingItems.ContainsKey(rigidBody)) return;

        _attractingItems.Add(rigidBody, item);
    }

    private void TryRemoveItem(Collider collider)
    {
        var rigidBody = collider.attachedRigidbody;
        if (rigidBody == null) return;

        _attractingItems.Remove(rigidBody);
    }

    private void AttractItems()
    {
        if (_attractingItems.Count == 0) return;

        Vector3 targetPosition = transform.position;

        List<Rigidbody> removeList = null;

        foreach (var pair in _attractingItems)
        {
            var rigidBody = pair.Key;
            var item = pair.Value;

            if (rigidBody == null || item == null || !item.IsThrown)
            {
                removeList ??= new List<Rigidbody>();
                removeList.Add(rigidBody);
                continue;
            }

            Vector3 direction = (targetPosition - rigidBody.position).normalized;
            rigidBody.AddForce(direction * _magnetForce, ForceMode.Acceleration);

            if (rigidBody.linearVelocity.sqrMagnitude > _maxSpeed * _maxSpeed)
            {
                rigidBody.linearVelocity = rigidBody.linearVelocity.normalized * _maxSpeed;
            }
        }

        if (removeList != null)
        {
            foreach (var rigidBody in removeList)
            {
                _attractingItems.Remove(rigidBody);
            }
        }
    }
}
