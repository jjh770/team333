using UnityEngine;

/// <summary>
/// 플레이어 기본 스탯 데이터
/// 체력, 방어력 등 캐릭터의 기본 능력치
/// </summary>
[CreateAssetMenu(menuName = "Game/Player/StatsData", fileName = "PlayerStatsData")]
public class PlayerStatsData : ScriptableObject
{
    [Header("Health")]
    [field: SerializeField] public float MaxHealth { get; private set; } = 100f;

}
