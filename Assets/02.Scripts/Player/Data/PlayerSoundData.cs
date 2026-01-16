using UnityEngine;

[CreateAssetMenu(menuName = "Game/Sound/PlayerSoundData", fileName = "PlayerSoundData")]
public class PlayerSoundData : ScriptableObject
{
    [Header("Attack")]
    [field: SerializeField]
    [Tooltip("기본 공격 사운드")]
    public AudioClip[] AttackSounds { get; private set; }

    [field: SerializeField]
    [Tooltip("각 공격 사운드의 시작 시간 (초)")]
    public float[] AttackSoundStartTimes { get; private set; }

    [Header("Hit")]
    [field: SerializeField]
    [Tooltip("피격 사운드")]
    public AudioClip[] HitSounds { get; private set; }

    [field: SerializeField]
    [Tooltip("사망 사운드")]
    public AudioClip DeathSFX { get; private set; }

    [Header("Movement")]
    [field: SerializeField]
    [Tooltip("발걸음 사운드")]
    public AudioClip[] FootstepSounds { get; private set; }

    [field: SerializeField]
    [Tooltip("대쉬 사운드")]
    public AudioClip DashSFX { get; private set; }

    [Header("Skill")]
    [field: SerializeField]
    [Tooltip("스킬 사용 사운드")]
    public AudioClip[] SkillSounds { get; private set; }

    public AudioClip GetAttackSound(int comboIndex)
    {
        if (AttackSounds == null || AttackSounds.Length == 0) return null;
        return AttackSounds[Mathf.Clamp(comboIndex, 0, AttackSounds.Length - 1)];
    }

    public float GetAttackSoundStartTime(int comboIndex)
    {
        if (AttackSoundStartTimes == null || AttackSoundStartTimes.Length == 0) return 0f;
        return AttackSoundStartTimes[Mathf.Clamp(comboIndex, 0, AttackSoundStartTimes.Length - 1)];
    }

    public AudioClip GetRandomHitSound()
    {
        if (HitSounds == null || HitSounds.Length == 0) return null;
        return HitSounds[Random.Range(0, HitSounds.Length)];
    }

    public AudioClip GetRandomFootstepSound()
    {
        if (FootstepSounds == null || FootstepSounds.Length == 0) return null;
        return FootstepSounds[Random.Range(0, FootstepSounds.Length)];
    }
}
