using UnityEngine;
using MoreMountains.Tools;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Volume Settings")]
    [SerializeField, Range(0f, 1f)] private float _bgmVolume = 0.5f;
    [SerializeField, Range(0f, 1f)] private float _sfxVolume = 1f;

    public float BGMVolume => _bgmVolume;
    public float SFXVolume => _sfxVolume;

    private AudioClip _currentBGM;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    #region BGM

    /// <summary>
    /// BGM 재생
    /// </summary>
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;

        // 같은 BGM이면 재생 안함
        if (_currentBGM == clip) return;

        _currentBGM = clip;

        MMSoundManagerPlayOptions options = MMSoundManagerPlayOptions.Default;
        options.MmSoundManagerTrack = MMSoundManager.MMSoundManagerTracks.Music;
        options.Location = transform.position;
        options.Loop = loop;
        options.Volume = _bgmVolume;

        MMSoundManagerSoundPlayEvent.Trigger(clip, options);
    }

    /// <summary>
    /// BGM 정지
    /// </summary>
    public void StopBGM()
    {
        _currentBGM = null;
        MMSoundManagerTrackEvent.Trigger(
            MMSoundManagerTrackEventTypes.StopTrack,
            MMSoundManager.MMSoundManagerTracks.Music
        );
    }

    /// <summary>
    /// BGM 페이드 아웃 후 정지
    /// </summary>
    public void FadeOutBGM(float duration = 1f)
    {
        _currentBGM = null;
        MMTweenType tween = new MMTweenType(MMTween.MMTweenCurve.EaseOutQuadratic);
        MMSoundManagerTrackFadeEvent.Trigger(
            MMSoundManagerTrackFadeEvent.Modes.PlayFade,
            MMSoundManager.MMSoundManagerTracks.Music,
            duration,
            0f,
            tween
        );
    }

    #endregion

    #region SFX

    public void PlaySFX(AudioClip clip, float volumeMultiplier = 1f)
    {
        PlaySFX(clip, 0f, volumeMultiplier);
    }

    public void PlaySFX(AudioClip clip, float startTime, float volumeMultiplier)
    {
        if (clip == null) return;

        MMSoundManagerPlayOptions options = MMSoundManagerPlayOptions.Default;
        options.MmSoundManagerTrack = MMSoundManager.MMSoundManagerTracks.Sfx;
        options.Location = transform.position;
        options.Volume = _sfxVolume * volumeMultiplier;
        options.PlaybackTime = startTime;

        MMSoundManagerSoundPlayEvent.Trigger(clip, options);
    }

    public void PlayRandomSFX(AudioClip[] clips, float volumeMultiplier = 1f)
    {
        if (clips == null || clips.Length == 0) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        PlaySFX(clip, volumeMultiplier);
    }

    #endregion

    #region Volume Control

    public void SetBGMVolume(float volume)
    {
        _bgmVolume = Mathf.Clamp01(volume);
        MMSoundManagerTrackEvent.Trigger(
            MMSoundManagerTrackEventTypes.SetVolumeTrack,
            MMSoundManager.MMSoundManagerTracks.Music,
            _bgmVolume
        );
    }

    public void SetSFXVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp01(volume);
        MMSoundManagerTrackEvent.Trigger(
            MMSoundManagerTrackEventTypes.SetVolumeTrack,
            MMSoundManager.MMSoundManagerTracks.Sfx,
            _sfxVolume
        );
    }
    
    public void MuteAll(bool mute)
    {
        MMSoundManagerTrackEvent.Trigger(
            mute ? MMSoundManagerTrackEventTypes.MuteTrack : MMSoundManagerTrackEventTypes.UnmuteTrack,
            MMSoundManager.MMSoundManagerTracks.Master
        );
    }

    #endregion
}
