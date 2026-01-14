using System.Collections.Generic;
using UnityEngine;

public class FloraMagnet : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _magnetForce = 10f;
    [SerializeField] private float _maxSpeed = 8f;

    private readonly HashSet<Rigidbody> _attractingItems = new HashSet<Rigidbody>();

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

    private void TryAddItem(Collider col)
    {
        if (!IsValidItem(col.gameObject)) return;

        var rb = col.attachedRigidbody;
        if (rb == null) return;

        var item = col.GetComponent<ItemBase>();
        if (item == null) return;

        if (!item.IsThrown) return;

        _attractingItems.Add(rb);
    }

    private void TryRemoveItem(Collider col)
    {
        var rb = col.attachedRigidbody;
        if (rb != null)
        {
            _attractingItems.Remove(rb);
        }
    }

    private bool IsValidItem(GameObject obj)
    {
        return obj.GetComponent<Wood>() != null ||
               obj.GetComponent<FloraSkillChanger>() != null;
    }

    private void AttractItems()
    {
        if (_attractingItems.Count == 0) return;

        Vector3 targetPos = transform.position;
        _attractingItems.RemoveWhere(rigidBody => rigidBody == null);

        foreach (var rigidBody in _attractingItems)
        {
            var item = rigidBody.GetComponent<ItemBase>();
            if (item == null) continue;

            if (!item.IsThrown) continue;

            Vector3 direction = (targetPos - rigidBody.position).normalized;
            rigidBody.AddForce(direction * _magnetForce, ForceMode.Acceleration);

            if (rigidBody.linearVelocity.magnitude > _maxSpeed)
            {
                rigidBody.linearVelocity = rigidBody.linearVelocity.normalized * _maxSpeed;
            }
        }
    }
}
