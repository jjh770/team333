using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Player/SkillData", fileName = "PlayerSkillData")]
public class PlayerSkillData : ScriptableObject
{
    [Header("Skill")]
    [field: SerializeField] public float SkillDamage { get; private set; } = 50f;
    [field: SerializeField] public float SkillRange { get; private set; } = 5f;

    [Header("Skill Movement")]
    [field: SerializeField] public float SkillMaxDistance { get; private set; } = 5f;
    [field: SerializeField] public float SkillJumpHeight { get; private set; } = 3f;
    [field: SerializeField] public Ease SkillMoveEase { get; private set; }

    [Header("Projectile")]
    [field: SerializeField] public float ProjectileSpeed { get; private set; } = 15f;
    [field: SerializeField] public float ProjectileRange { get; private set; } = 10f;
    [field: SerializeField] public float ProjectileWidth { get; private set; } = 1.5f;
    [field: SerializeField] public float ProjectileHeight { get; private set; } = 1f;
    [field: SerializeField] public float ProjectileDepth { get; private set; } = 0.5f;
}
