using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Player/SkillData", fileName = "PlayerSkillData")]
public class PlayerSkillData : ScriptableObject
{
    [Header("Skill")]
    [field: SerializeField] public float SkillDamage { get; private set; } = 50f;
    [field: SerializeField] public float SkillRange { get; private set; } = 5f;
    [field: SerializeField] public float Cooldown { get; private set; } = 3f;

    [Header("Skill Level")]
    [field: SerializeField] public int MaxSkillLevel { get; private set; } = 3;

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

    [Header("Projectile Spread")]
    [field: SerializeField] public float[] SingleProjectileAngles { get; private set; } = { 0f };
    [field: SerializeField] public float[] TripleProjectileAngles { get; private set; } = { -45f, 0f, 45f };

    public void SetupProjectile(SkillProjectile projectile, Vector3 direction, GameObject owner, LayerMask monsterLayers)
    {
        var config = new ProjectileConfig
        {
            Direction = direction,
            Speed = ProjectileSpeed,
            MaxDistance = ProjectileRange,
            Width = ProjectileWidth,
            Height = ProjectileHeight,
            Depth = ProjectileDepth,
            Damage = SkillDamage,
            Owner = owner,
            MonsterLayers = monsterLayers
        };
        projectile.Initialize(config);
    }
}

