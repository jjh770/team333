using UnityEngine;

public class MonsterSound : MonoBehaviour
{
    [SerializeField] private MonsterSoundData _soundData;

    public void Hit()
    {
        if (_soundData == null) return;
        PlaySoundInfo(_soundData.GetRandomHitSound());
    }

    public void PetAppear()
    {
        if (_soundData == null) return;
        PlaySoundInfo(_soundData.PetAppearSFX);
    }

    public void PetAttack()
    {
        if (_soundData == null) return;
        PlaySoundInfo(_soundData.PetAttackSFX);
    }

    public void EatSansam()
    {
        if (_soundData == null) return;
        PlaySoundInfo(_soundData.EatSansamSFX);
    }

    private void PlaySoundInfo(MonsterSoundInfo info)
    {
        if (info.Clip == null) return;
        SoundManager.Instance.PlaySFX(info.Clip, info.StartTime, 1f);
    }
}
