using UnityEngine;

public class UI_EndSceneSound : MonoBehaviour
{
    [Header("Panel Sounds")]
    [SerializeField] private SoundInfo _woodDropSFX;
    [SerializeField] private SoundInfo _inputPanelShowSFX;
    [SerializeField] private SoundInfo _leaderboardPanelShowSFX;
    [SerializeField] private SoundInfo _stampSFX;
    [SerializeField] private SoundInfo _buttonHoverSFX;
    [SerializeField] private float _buttonHoverSFXVolume = 0.1f;
        
    [Header("Rank Sounds")]
    [SerializeField] private SoundInfo _rankCountingSFX;
    [SerializeField] private SoundInfo _firstRankParticleSFX;

    [Header("Settings")]
    [SerializeField] private float _volumeMultiplier = 1f;

    public void PlayWoodDrop()
    {
        PlaySound(_woodDropSFX);
    }

    public void PlayInputPanelShow()
    {
        PlaySound(_inputPanelShowSFX);
    }

    public void PlayLeaderboardPanelShow()
    {
        PlaySound(_leaderboardPanelShowSFX);
    }

    public void PlayStamp()
    {
        PlaySound(_stampSFX);
    }

    public void PlayButtonHover()
    {
        PlaySound(_buttonHoverSFX, _buttonHoverSFXVolume);
    }

    public void PlayRankCounting()
    {
        PlaySound(_rankCountingSFX);
    }

    public void PlayFirstRankParticle()
    {
        PlaySound(_firstRankParticleSFX);
    }

    private void PlaySound(SoundInfo info, float? volume = null)
    {
        if (info.Clip == null) return;
        SoundManager.Instance?.PlaySFX(info.Clip, info.StartTime, volume ?? _volumeMultiplier);
    }
}