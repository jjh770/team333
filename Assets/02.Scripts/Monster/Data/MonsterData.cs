using UnityEngine;

[CreateAssetMenu(menuName = "Game/Monster/MonsterData")]
public class MonsterData : ScriptableObject
{
    [Header("Identity")]
    [field: SerializeField] public string MonsterName { get; private set; }
}
