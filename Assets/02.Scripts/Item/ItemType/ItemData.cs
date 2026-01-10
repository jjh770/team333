using UnityEngine;

public enum ItemType
{
    HealthUp,
    Wood,
    Board
}

public class ItemData : ScriptableObject
{
    [Header("Type")]
    [field: SerializeField] public ItemType Type { get; private set; }
    
    [Header("HealthUp")]
    [field: SerializeField] public float HealthUpAmount { get; private set; } = 20f;
}
