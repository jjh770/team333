using UnityEngine;

/// <summary>
/// 플레이어 대시 관련 데이터
/// 대시 속도, 지속 시간, 쿨다운
/// </summary>
[CreateAssetMenu(menuName = "Game/Player/DashData", fileName = "PlayerDashData")]
public class PlayerDashData : ScriptableObject
{
    [Header("Dash Settings")]
    [field: SerializeField]
    [Tooltip("대시 속도")]
    public float DashSpeed { get; private set; } = 10f;

    [field: SerializeField]
    [Tooltip("대시 지속 시간 (초)")]
    public float DashDuration { get; private set; } = 0.5f;

    [field: SerializeField]
    [Tooltip("대시 쿨다운 시간 (초)")]
    public float DashCoolDown { get; private set; } = 1f;

    // 향후 확장 가능
    // [field: SerializeField] public int MaxDashCharges { get; private set; } = 1;
    // [field: SerializeField] public bool CanDashInAir { get; private set; } = false;
}
