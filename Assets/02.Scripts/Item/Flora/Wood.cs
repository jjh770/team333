using UnityEngine;

public class Wood : ItemBase
{
    [Header("Settings")]
    [SerializeField] private int _addWoodAmount = 1;

    private const string FloraTag = "Flora";

    private void OnCollisionEnter(Collision other)
    {
        if (_isHeld) return;

        if (!other.gameObject.CompareTag(FloraTag)) return;
        if (!other.gameObject.TryGetComponent<FloraInteraction>(out var interaction)) return;

        Debug.Log("OnCollisionEnter");
        interaction.AddWood(_addWoodAmount);
        ItemFactory.Instance.Despawn(this.gameObject);
    }
}
