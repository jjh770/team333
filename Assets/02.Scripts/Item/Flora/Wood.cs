using UnityEngine;

public class Wood : ItemBase
{
    [Header("Settings")]
    [SerializeField] private int _addWoodAmount = 1;
    
    public override bool CanThrow => true;

    private const string FloraTag = "Flora";

    private void OnCollisionEnter(Collision other)
    {
        if (_isHeld) return;
        
        if (!other.gameObject.CompareTag(FloraTag)) return;
        if (!other.gameObject.TryGetComponent<FloraInteraction>(out var interaction)) return;

        interaction.AddWood(_addWoodAmount);
        Destroy(gameObject);
    }
}
