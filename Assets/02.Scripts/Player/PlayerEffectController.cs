using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerEffectHandler
{
    public GameObject EffectObject;

    public void Play()
    {
        if (EffectObject != null)
        {
            EffectObject.SetActive(false);
            EffectObject.SetActive(true);
        }
    }

    public void Stop()
    {
        if (EffectObject != null)
        {
            EffectObject.SetActive(false);
        }
    }
}

public class PlayerEffectController : MonoBehaviour
{
    [Header("Attack Effects")]
    [SerializeField] private List<PlayerEffectHandler> _slashEffects;

    [Header("Skill Effects")]
    [SerializeField] private PlayerEffectHandler _skillEffect;

    [Header("Item Effects")]
    [SerializeField] private PlayerEffectHandler _healEffect;
    [SerializeField] private PlayerEffectHandler _skillBoostEffect;

    private void OnEnable()
    {
        StopAllEffects();
    }

    public void StopAllEffects()
    {
        foreach (var effect in _slashEffects)
        {
            effect?.Stop();
        }
        _skillEffect?.Stop();
        _healEffect?.Stop();
        _skillBoostEffect?.Stop();
    }

    #region Attack Effects

    public void PlaySlash(int comboIndex)
    {
        StopAllSlashes();
        if (comboIndex >= 0 && comboIndex < _slashEffects.Count)
        {
            _slashEffects[comboIndex]?.Play();
        }
    }

    public void StopAllSlashes()
    {
        foreach (var effect in _slashEffects)
        {
            effect?.Stop();
        }
    }

    #endregion

    #region Skill Effects

    public void PlaySkill()
    {
        _skillEffect?.Play();
    }

    public void StopSkill()
    {
        _skillEffect?.Stop();
    }

    #endregion

    #region Item Effects

    public void PlayHeal()
    {
        _healEffect?.Play();
    }

    public void StopHeal()
    {
        _healEffect?.Stop();
    }

    public void PlaySkillUp()
    {
        _skillBoostEffect?.Play();
    }

    public void StopSkillUp()
    {
        _skillBoostEffect?.Stop();
    }

    #endregion
}
