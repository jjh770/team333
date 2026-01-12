using UnityEngine;

public class Plank : ItemBase
{
    private const string QuestBridgeTag = "QuestBridge";
    private ItemFactory _itemFactory;

    override protected void Awake()
    {
        base.Awake();
        _itemFactory = ItemFactory.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isHeld) return;
        if (!other.CompareTag(QuestBridgeTag)) return;

        if (other.TryGetComponent<BridgeQuest>(out var bridgeQuest))
        {
            bridgeQuest.AddPlank();
        }

        _itemFactory.Despawn(this.gameObject);
    }
}
