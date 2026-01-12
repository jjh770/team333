using DG.Tweening;
using UnityEngine;

/// <summary>
/// 플레이어 공격 관련 데이터
/// 공격 데미지, 범위, 콤보 시스템, 공격 중 이동 설정
/// </summary>
[CreateAssetMenu(menuName = "Game/Player/AttackData", fileName = "PlayerAttackData")]
public class PlayerAttackData : ScriptableObject
{
    [Header("Basic Attack")]
    [field: SerializeField] public float AttackDamage { get; private set; } = 10f;
    [field: SerializeField] public float AttackRange { get; private set; } = 2f;
    [field: SerializeField]
    [Tooltip("각 콤보별 공격 각도 (90도 = 전방 90도 부채꼴)")]
    public float[] AttackAngle { get; private set; } = { 90f, 120f, 150f };

    [field: SerializeField]
    [Tooltip("각 콤보별 공격 방향 오프셋 (0 = 정면, 양수 = 우측, 음수 = 좌측)")]
    public float[] AttackDirectionOffsets { get; private set; } = { 0f, 0f, 0f };

    [Header("Combo System")]
    [field: SerializeField]
    [Tooltip("최대 콤보 단계 (3 = 1타, 2타, 3타)")]
    public int MaxComboCount { get; private set; } = 3;

    [field: SerializeField]
    [Tooltip("콤보 윈도우 시간 (초 단위)")]
    public float ComboWindowTime { get; private set; } = 1f;

    [field: SerializeField]
    [Tooltip("각 콤보별 데미지 배율 (1.0 = 기본, 1.5 = 150%)")]
    public float[] ComboDamageMultipliers { get; private set; } = { 1f, 1.2f, 1.5f };

    [Header("Attack Movement")]
    [field: SerializeField]
    [Tooltip("각 콤보별 돌진 이동 거리")]
    public float[] AttackMoveDistance { get; private set; } = { 1f, 1.5f, 2f };

    [field: SerializeField]
    [Tooltip("각 콤보별 이동 Ease 효과")]
    public Ease[] AttackMoveEase { get; private set; } = {
        Ease.OutQuad,
        Ease.OutCubic,
        Ease.OutQuint
    };

    // 유효성 검증
    private void OnValidate()
    {
        ValidateArrayLength(AttackAngle, nameof(AttackAngle));
        ValidateArrayLength(AttackDirectionOffsets, nameof(AttackDirectionOffsets));
        ValidateArrayLength(ComboDamageMultipliers, nameof(ComboDamageMultipliers));
        ValidateArrayLength(AttackMoveDistance, nameof(AttackMoveDistance));
        ValidateArrayLength(AttackMoveEase, nameof(AttackMoveEase));
    }

    private void ValidateArrayLength(System.Array array, string arrayName)
    {
        if (array.Length != MaxComboCount)
        {
            Debug.LogWarning($"{arrayName} 배열의 크기({array.Length})가 MaxComboCount({MaxComboCount})와 다릅니다.");
        }
    }
}
