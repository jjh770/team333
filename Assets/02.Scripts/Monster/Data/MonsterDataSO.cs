using UnityEngine;

[CreateAssetMenu(menuName = "Game/Monster/MonsterDataSO")]
public class MonsterDataSO : ScriptableObject
{
    [Header("Identity")]
    [field: SerializeField] public string MonsterName { get; private set; }

    [Header("Stats")]
    [field: SerializeField] public float MaxHealth { get; private set; } = 100f;
    [field: SerializeField] public float AttackDamage { get; private set; } = 10f;
    [field: SerializeField] public float AttackCooltime { get; private set; } = 1.5f;
    [field: SerializeField] public float AttackDistance { get; private set; } = 2.5f;
    [field: SerializeField] public float AttackDuration { get; private set; } = 0.14f;
    [field: SerializeField] public float MoveSpeed { get; private set; } = 4f;

    [Header("DropItem")]
    [field: SerializeField] public GameObject DropItem { get; private set; }

    [Header("Prefab")]
    [field: SerializeField] public GameObject Prefab { get; private set; }
}
