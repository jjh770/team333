using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private PlayerSoundData _soundData;

    #region Attack

    public void PlayAttack(int comboIndex)
    {
        if (_soundData == null) return;

        var clip = _soundData.GetAttackSound(comboIndex);
        float startTime = _soundData.GetAttackSoundStartTime(comboIndex);
        SoundManager.Instance.PlaySFX(clip, startTime, 1f);
    }

    #endregion

    #region Hit

    public void PlayHit()
    {
        if (_soundData == null) return;

        var clip = _soundData.GetRandomHitSound();
        SoundManager.Instance.PlaySFX(clip);
    }

    public void PlayDeath()
    {
        if (_soundData == null) return;

        SoundManager.Instance.PlaySFX(_soundData.DeathSFX);
    }

    #endregion

    #region Movement

    public void PlayFootstep()
    {
        if (_soundData == null) return;

        var clip = _soundData.GetRandomFootstepSound();
        SoundManager.Instance.PlaySFX(clip);
    }

    public void PlayDash()
    {
        if (_soundData == null) return;

        SoundManager.Instance.PlaySFX(_soundData.DashSFX);
    }

    #endregion

    #region Skill

    public void PlaySkill(int skillIndex = 0)
    {
        if (_soundData == null || _soundData.SkillSounds == null || _soundData.SkillSounds.Length == 0) return;

        int index = Mathf.Clamp(skillIndex, 0, _soundData.SkillSounds.Length - 1);
        SoundManager.Instance.PlaySFX(_soundData.SkillSounds[index]);
    }

    #endregion
}
