using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private PlayerSoundData _soundData;

    private PlayerStateManager _stateManager;

    private void Awake()
    {
        _stateManager = GetComponent<PlayerStateManager>();
    }

    #region Attack

    public void PlayAttack(int comboIndex)
    {
        if (_soundData == null) return;

        var info = _soundData.GetAttackSound(comboIndex);
        SoundManager.Instance.PlaySFX(info.Clip, info.StartTime, 1f);
    }

    #endregion

    #region Hit

    public void PlayHit()
    {
        if (_soundData == null) return;

        var info = _soundData.GetRandomHitSound();
        SoundManager.Instance.PlaySFX(info.Clip, info.StartTime, 1f);
    }

    public void PlayDeath()
    {
        if (_soundData == null) return;

        var info = _soundData.DeathSFX;
        SoundManager.Instance.PlaySFX(info.Clip, info.StartTime, 1f);
    }

    #endregion

    #region Movement

    // 애니메이션 이벤트용
    public void OnFootstep()
    {
        if (_stateManager != null && !_stateManager.IsState(PlayerState.Moving)) return;

        PlayFootstep();
    }

    public void PlayFootstep()
    {
        if (_soundData == null) return;

        var info = _soundData.GetRandomFootstepSound();
        SoundManager.Instance.PlaySFX(info.Clip, info.StartTime, 1f);
    }

    public void PlayDash()
    {
        if (_soundData == null) return;

        var info = _soundData.DashSFX;
        SoundManager.Instance.PlaySFX(info.Clip, info.StartTime, 1f);
    }

    #endregion

    #region Skill

    public void PlaySkill(int skillIndex = 0)
    {
        if (_soundData == null || _soundData.SkillSounds == null || _soundData.SkillSounds.Length == 0) return;

        int index = Mathf.Clamp(skillIndex, 0, _soundData.SkillSounds.Length - 1);
        var info = _soundData.SkillSounds[index];
        SoundManager.Instance.PlaySFX(info.Clip, info.StartTime, 1f);
    }

    #endregion

    #region Item

    public void PlayItemConsume()
    {
        if (_soundData == null) return;

        var info = _soundData.ItemConsumeSFX;
        SoundManager.Instance.PlaySFX(info.Clip, info.StartTime, 1f);
    }

    public void PlayItemHold()
    {
        if (_soundData == null) return;

        var info = _soundData.ItemHoldSFX;
        SoundManager.Instance.PlaySFX(info.Clip, info.StartTime, 1f);
    }

    public void PlayItemThrow()
    {
        if (_soundData == null) return;

        var info = _soundData.ItemThrowSFX;
        SoundManager.Instance.PlaySFX(info.Clip, info.StartTime, 1f);
    }

    #endregion
}
