using UnityEngine;

[System.Serializable]
public struct AttackSoundInfo
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
    public AttackSoundInfo[] AttackSounds { get; private set; }

    [Header("Hit")]
    [field: SerializeField]
    [Tooltip("피격 사운드")]
    public AttackSoundInfo[] HitSounds { get; private set; }

    [field: SerializeField]
    [Tooltip("사망 사운드")]
    public AttackSoundInfo DeathSFX { get; private set; }

    [Header("Movement")]
    [field: SerializeField]
    [Tooltip("발걸음 사운드")]
    public AttackSoundInfo[] FootstepSounds { get; private set; }

    [field: SerializeField]
    [Tooltip("대쉬 사운드")]
    public AttackSoundInfo DashSFX { get; private set; }

    [Header("Skill")]
    [field: SerializeField]
    [Tooltip("스킬 사용 사운드")]
    public AttackSoundInfo[] SkillSounds { get; private set; }

    public AudioClip GetAttackSound(int comboIndex)
    {
        if (AttackSounds == null || AttackSounds.Length == 0) return null;
        return AttackSounds[Mathf.Clamp(comboIndex, 0, AttackSounds.Length - 1)].Clip;
    }

    public float GetAttackSoundStartTime(int comboIndex)
    {
        if (AttackSounds == null || AttackSounds.Length == 0) return 0f;
        return AttackSounds[Mathf.Clamp(comboIndex, 0, AttackSounds.Length - 1)].StartTime;
    }

    public AudioClip GetRandomHitSound()
    {
        if (HitSounds == null || HitSounds.Length == 0) return null;
        return HitSounds[Random.Range(0, HitSounds.Length)].Clip;
    }

    public AudioClip GetRandomFootstepSound()
    {
        if (FootstepSounds == null || FootstepSounds.Length == 0) return null;
        return FootstepSounds[Random.Range(0, FootstepSounds.Length)].Clip;
    }
}
