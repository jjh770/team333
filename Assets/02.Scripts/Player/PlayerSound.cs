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
        PlaySoundInfo(_soundData.GetAttackSound(comboIndex));
    }

    #endregion

    #region Hit

    public void PlayHit()
    {
        if (_soundData == null) return;
        PlaySoundInfo(_soundData.GetRandomHitSound());
    }

    public void PlayDeath()
    {
        if (_soundData == null) return;
        PlaySoundInfo(_soundData.DeathSFX);
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
        PlaySoundInfo(_soundData.GetRandomFootstepSound());
    }

    public void PlayDash()
    {
        if (_soundData == null) return;
        PlaySoundInfo(_soundData.DashSFX);
    }

    #endregion

    #region Skill

    public void PlaySkill(int skillIndex = 0)
    {
        if (_soundData == null || _soundData.SkillSounds == null || _soundData.SkillSounds.Length == 0) return;

        int index = Mathf.Clamp(skillIndex, 0, _soundData.SkillSounds.Length - 1);
        PlaySoundInfo(_soundData.SkillSounds[index]);
    }

    #endregion

    #region Item

    public void PlayItemConsume()
    {
        if (_soundData == null) return;
        PlaySoundInfo(_soundData.ItemConsumeSFX);
    }

    public void PlayItemHold()
    {
        if (_soundData == null) return;
        PlaySoundInfo(_soundData.ItemHoldSFX);
    }

    public void PlayItemThrow()
    {
        if (_soundData == null) return;
        PlaySoundInfo(_soundData.ItemThrowSFX);
    }

    #endregion

    private void PlaySoundInfo(AttackSoundInfo info)
    {
        if (info.Clip == null) return;
        SoundManager.Instance.PlaySFX(info.Clip, info.StartTime, 1f);
    }
}
