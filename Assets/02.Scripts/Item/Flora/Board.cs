using UnityEngine;

public class Board : ItemBase
{
    private const string QuestBridgeTag = "QuestBridge";
    private ItemFactory _itemFactory;
    public override bool HidesFloraOutline => true;
    
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

        _itemFactory.ReturnItem(this.gameObject);
    }
}
