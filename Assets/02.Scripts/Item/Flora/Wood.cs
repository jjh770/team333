using UnityEngine;

public class Wood : ItemBase
{
    [Header("Settings")]
    [SerializeField] private int _addWoodAmount = 1;

    private const string FloraTag = "Flora";
    private ItemFactory _itemFactory;

    override protected void Awake()
    {
        base.Awake();
        _itemFactory = ItemFactory.Instance;
    }

    protected override void OnCollisionEnter(Collision other)
    {
        base.OnCollisionEnter(other);
        
        if (_isHeld) return;

        if (!other.gameObject.CompareTag(FloraTag)) return;
        if (!other.gameObject.TryGetComponent<FloraInteraction>(out var interaction)) return;

        interaction.AddWood(_addWoodAmount);
        _itemFactory.ReturnItem(this.gameObject);
    }
}
