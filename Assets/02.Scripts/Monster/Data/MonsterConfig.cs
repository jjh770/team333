using UnityEngine;

[CreateAssetMenu(menuName = "Game/Monster/MonsterConfig")]
public class MonsterConfig : ScriptableObject
{
    [Header("Identity")]
    public string MonsterName;

    /*    
    // 드랍 아이템
    [Header("Drops")]
    public DropTable DropTable;

    // 프리팹 (어드레서블)
    [Header("Prefab")]
    public AssetReferenceGameObject prefabRef;
    */
}
