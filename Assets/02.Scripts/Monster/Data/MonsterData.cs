using UnityEngine;

[CreateAssetMenu(menuName = "Game/Monster/MonsterData")]
public class MonsterData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string _monsterName;
}
