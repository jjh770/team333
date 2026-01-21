using UnityEngine;

public class FloraSound : MonoBehaviour
{
    [SerializeField] private FloraSoundData _soundData;

    public void PlaySpeedUp()
    {
        if (_soundData == null) return;
        PlaySoundInfo(_soundData.SpeedUpSFX);
    }

    public void PlayInteraction()
    {
        if (_soundData == null) return;
        PlaySoundInfo(_soundData.GetRandomInteractionSound());
    }

    public void PlayGetItem()
    {
        if (_soundData == null) return;
        PlaySoundInfo(_soundData.GetItemSound);
    }

    public void PlayBridgeQuestSound()
    {
        if (_soundData == null) return;
        PlaySoundInfo(_soundData.BridgeQuestSound);
    }

    public void PlayQuestComplete()
    {
        if (_soundData == null) return;
        PlaySoundInfo(_soundData.QuestCompleteSound);
    }

    public void PlayHit()
    {
        if (_soundData == null) return;
        PlaySoundInfo(_soundData.GetRandomHitSound());
    }

    private void PlaySoundInfo(SoundInfo info)
    {
        if (info.Clip == null) return;
        SoundManager.Instance.PlaySFX(info.Clip, info.StartTime, 1f);
    }
}
