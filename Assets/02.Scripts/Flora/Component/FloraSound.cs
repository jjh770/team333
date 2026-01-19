using UnityEngine;

public class FloraSound : MonoBehaviour
{
    [SerializeField] private FloraSoundData _soundData;

    public void PlaySpeedUp()
    {
        if (_soundData == null) return;
        PlaySoundInfo(_soundData.SpeedUpSFX);
    }

    private void PlaySoundInfo(SoundInfo info)
    {
        if (info.Clip == null) return;
        SoundManager.Instance.PlaySFX(info.Clip, info.StartTime, 1f);
    }
}
