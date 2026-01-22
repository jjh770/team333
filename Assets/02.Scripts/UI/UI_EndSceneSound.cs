using UnityEngine;

public class UI_EndSceneSound : MonoBehaviour
{
    [Header("Panel Sounds")]
    [SerializeField] private SoundInfo _woodDropSFX;
    [SerializeField] private SoundInfo _inputPanelShowSFX;
    [SerializeField] private SoundInfo _leaderboardPanelShowSFX;
    [SerializeField] private SoundInfo _stampSFX;

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

    private void PlaySound(SoundInfo info)
    {
        if (info.Clip == null) return;
        SoundManager.Instance?.PlaySFX(info.Clip, info.StartTime, _volumeMultiplier);
    }
}