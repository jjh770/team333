using UnityEngine;
using MoreMountains.Tools;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Volume Settings")]
    [SerializeField, Range(0f, 1f)] private float _bgm1Volume = 0.5f;
    [SerializeField, Range(0f, 1f)] private float _bgm2Volume = 0.5f;
    [SerializeField, Range(0f, 1f)] private float _sfxVolume = 1f;

    [Header("BGM")]
    [SerializeField] private AudioClip _startBGM1;
    [SerializeField] private AudioClip _startBGM2;

    public float BGM1Volume => _bgm1Volume;
    public float BGM2Volume => _bgm2Volume;
    public float SFXVolume => _sfxVolume;

    private AudioSource _bgm1Source;
    private AudioSource _bgm2Source;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _bgm1Source = gameObject.AddComponent<AudioSource>();
        _bgm1Source.loop = true;
        _bgm1Source.playOnAwake = false;

        _bgm2Source = gameObject.AddComponent<AudioSource>();
        _bgm2Source.loop = true;
        _bgm2Source.playOnAwake = false;
    }

    private void Start()
    {
        if (_startBGM1 != null)
        {
            PlayBGM1(_startBGM1);
        }
        if (_startBGM2 != null)
        {
            PlayBGM2(_startBGM2);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    #region BGM1

    public void PlayBGM1(AudioClip clip)
    {
        if (clip == null) return;
        if (_bgm1Source.clip == clip && _bgm1Source.isPlaying) return;

        _bgm1Source.clip = clip;
        _bgm1Source.volume = _bgm1Volume;
        _bgm1Source.Play();
    }

    public void StopBGM1()
    {
        _bgm1Source.Stop();
        _bgm1Source.clip = null;
    }

    public void SetBGM1Volume(float volume)
    {
        _bgm1Volume = Mathf.Clamp01(volume);
        _bgm1Source.volume = _bgm1Volume;
    }

    #endregion

    #region BGM2

    public void PlayBGM2(AudioClip clip)
    {
        if (clip == null) return;
        if (_bgm2Source.clip == clip && _bgm2Source.isPlaying) return;

        _bgm2Source.clip = clip;
        _bgm2Source.volume = _bgm2Volume;
        _bgm2Source.Play();
    }

    public void StopBGM2()
    {
        _bgm2Source.Stop();
        _bgm2Source.clip = null;
    }

    public void SetBGM2Volume(float volume)
    {
        _bgm2Volume = Mathf.Clamp01(volume);
        _bgm2Source.volume = _bgm2Volume;
    }

    #endregion

    #region BGM Common

    public void StopAllBGM()
    {
        StopBGM1();
        StopBGM2();
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
        _bgm1Source.mute = mute;
        _bgm2Source.mute = mute;
        MMSoundManagerTrackEvent.Trigger(
            mute ? MMSoundManagerTrackEventTypes.MuteTrack : MMSoundManagerTrackEventTypes.UnmuteTrack,
            MMSoundManager.MMSoundManagerTracks.Master
        );
    }

    #endregion
}