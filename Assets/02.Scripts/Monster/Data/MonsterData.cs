using UnityEngine;

[CreateAssetMenu(menuName = "Game/Monster/MonsterData")]
public class MonsterData : ScriptableObject
{
    [Header("Identity")]
    [field: SerializeField] public string MonsterName { get; private set; }

    [Header("Stats")]
    [field: SerializeField] public float MaxHealth { get; private set; } = 100f;
    [field: SerializeField] public float AttackDamage { get; private set; } = 10f;
    [field: SerializeField] public float MoveSpeed { get; private set; } = 4f;
}
