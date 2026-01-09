using UnityEngine;

/// <summary>
/// 플레이어 이동 관련 데이터
/// 이동 속도, 회전 속도, 중력, 카메라 경계 설정
/// </summary>
[CreateAssetMenu(menuName = "Game/Player/MoveData", fileName = "PlayerMoveData")]
public class PlayerMoveData : ScriptableObject
{
    [Header("Movement")]
    [field: SerializeField]
    [Tooltip("기본 이동 속도")]
    public float MoveSpeed { get; private set; } = 5f;

    [field: SerializeField]
    [Tooltip("회전 속도 (부드러운 회전)")]
    public float RotationSpeed { get; private set; } = 10f;

    [Header("Physics")]
    [field: SerializeField]
    [Tooltip("중력 가속도")]
    public float Gravity { get; private set; } = -9.81f;

    [Header("Camera Boundary")]
    [field: SerializeField]
    [Tooltip("카메라 뷰포트 경계 마진 (0.0 ~ 1.0)")]
    [Range(0f, 0.2f)]
    public float ViewportMargin { get; private set; } = 0.05f;

    // 향후 확장 가능
    // [field: SerializeField] public float SprintSpeedMultiplier { get; private set; } = 1.5f;
    // [field: SerializeField] public float CrouchSpeedMultiplier { get; private set; } = 0.5f;
}
