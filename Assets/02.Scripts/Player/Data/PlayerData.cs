using UnityEngine;

/// <summary>
/// 플레이어 데이터 통합 컨테이너
/// 도메인별로 분리된 데이터들을 참조하는 루트 ScriptableObject
/// 필요 시 각 컴포넌트는 개별 데이터만 참조하거나, 이 컨테이너를 통해 접근 가능
/// </summary>
[CreateAssetMenu(menuName = "Game/Player/PlayerData", fileName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Player Data References")]
    [field: SerializeField]
    [Tooltip("기본 스탯 (체력, 방어력 등)")]
    public PlayerStatsData Stats { get; private set; }

    [field: SerializeField]
    [Tooltip("공격 관련 데이터 (데미지, 범위, 콤보 등)")]
    public PlayerAttackData Attack { get; private set; }

    [field: SerializeField]
    [Tooltip("대시 관련 데이터 (속도, 쿨다운 등)")]
    public PlayerDashData Dash { get; private set; }

    [field: SerializeField]
    [Tooltip("이동 관련 데이터 (속도, 회전 등)")]
    public PlayerMoveData Move { get; private set; }

    [field: SerializeField]
    [Tooltip("사운드 관련 데이터")]
    public PlayerSoundData Sound { get; private set; }

    [Header("Prefab")]
    [field: SerializeField]
    [Tooltip("플레이어 프리팹 참조")]
    public GameObject Prefab { get; private set; }

    // 유효성 검증
    private void OnValidate()
    {
        if (Stats == null)
            Debug.LogWarning($"{name}: Stats 데이터가 할당되지 않았습니다!", this);

        if (Attack == null)
            Debug.LogWarning($"{name}: Attack 데이터가 할당되지 않았습니다!", this);

        if (Dash == null)
            Debug.LogWarning($"{name}: Dash 데이터가 할당되지 않았습니다!", this);

        if (Move == null)
            Debug.LogWarning($"{name}: Move 데이터가 할당되지 않았습니다!", this);

        if (Sound == null)
            Debug.LogWarning($"{name}: Sound 데이터가 할당되지 않았습니다!", this);

        if (Prefab == null)
            Debug.LogWarning($"{name}: Prefab이 할당되지 않았습니다!", this);
    }
}
