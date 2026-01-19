using UnityEngine;

[System.Serializable]
public struct SoundInfo
{
    public AudioClip Clip;
    [Tooltip("사운드 시작 시간 (초)")]
    public float StartTime;
}

[CreateAssetMenu(menuName = "Game/Sound/PlayerSoundData", fileName = "PlayerSoundData")]
public class PlayerSoundData : ScriptableObject
{
    [Header("Attack")]
    [field: SerializeField]
    [Tooltip("기본 공격 사운드 정보 (클립, 시작 시간)")]
    public SoundInfo[] AttackSounds { get; private set; }

    [Header("Hit")]
    [field: SerializeField]
    [Tooltip("피격 사운드")]
    public SoundInfo[] HitSounds { get; private set; }

    [field: SerializeField]
    [Tooltip("사망 사운드")]
    public SoundInfo DeathSFX { get; private set; }

    [Header("Movement")]
    [field: SerializeField]
    [Tooltip("발걸음 사운드")]
    public SoundInfo[] FootstepSounds { get; private set; }

    [field: SerializeField]
    [Tooltip("대쉬 사운드")]
    public SoundInfo DashSFX { get; private set; }

    [Header("Skill")]
    [field: SerializeField]
    [Tooltip("스킬 사용 사운드")]
    public SoundInfo[] SkillSounds { get; private set; }

    [Header("Item")]
    [field: SerializeField]
    [Tooltip("소비 아이템 획득 사운드")]
    public SoundInfo ItemConsumeSFX { get; private set; }

    [field: SerializeField]
    [Tooltip("아이템 집기 사운드")]
    public SoundInfo ItemHoldSFX { get; private set; }

    [field: SerializeField]
    [Tooltip("아이템 던지기 사운드")]
    public SoundInfo ItemThrowSFX { get; private set; }

    [field: SerializeField]
    [Tooltip("하트비트 사운드")]
    public SoundInfo HearBeatSFX { get; private set; }

    public SoundInfo GetAttackSound(int comboIndex)
    {
        if (AttackSounds == null || AttackSounds.Length == 0) return default;
        return AttackSounds[Mathf.Clamp(comboIndex, 0, AttackSounds.Length - 1)];
    }

    public SoundInfo GetRandomHitSound()
    {
        if (HitSounds == null || HitSounds.Length == 0) return default;
        return HitSounds[Random.Range(0, HitSounds.Length)];
    }

    public SoundInfo GetRandomFootstepSound()
    {
        if (FootstepSounds == null || FootstepSounds.Length == 0) return default;
        return FootstepSounds[Random.Range(0, FootstepSounds.Length)];
    }
}
