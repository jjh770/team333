using UnityEngine;

[System.Serializable]
public struct MonsterSoundInfo
{
    public AudioClip Clip;
    [Tooltip("사운드 시작 시간 (초)")]
    public float StartTime;
}

[CreateAssetMenu(menuName = "Game/Sound/MonsterSoundData", fileName = "MonsterSoundData")]
public class MonsterSoundData : ScriptableObject
{
    [Header("Hit")]
    [field: SerializeField]
    [Tooltip("몬스터 피격 사운드")]
    public MonsterSoundInfo[] HitSounds { get; private set; }

    [field: SerializeField]
    [Tooltip("펫 등장 사운드")]
    public MonsterSoundInfo PetAppearSFX { get; private set; }

    [field: SerializeField]
    [Tooltip("펫 공격 사운드")]
    public MonsterSoundInfo PetAttackSFX { get; private set; }

    [field: SerializeField]
    [Tooltip("산삼 먹히는 사운드")]
    public MonsterSoundInfo EatSansamSFX { get; private set; }

    public MonsterSoundInfo GetRandomHitSound()
    {
        if (HitSounds == null || HitSounds.Length == 0) return default;
        return HitSounds[Random.Range(0, HitSounds.Length)];
    }
}
