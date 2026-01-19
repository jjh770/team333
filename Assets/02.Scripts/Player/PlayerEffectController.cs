using System.Collections.Generic;
using UnityEngine;

public enum PlayerEffectType
{
    Skill,
    Heal,
    SkillUp
}

[System.Serializable]
public class PlayerEffectHandler
{
    public GameObject EffectObject;

    private ParticleSystem _particleSystem;
    private bool _isCached;

    public void Play()
    {
        if (EffectObject == null) return;

        EffectObject.SetActive(true);
        CacheParticleSystem();

        if (_particleSystem != null)
        {
            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _particleSystem.Play(true);
        }
    }

    public void Stop()
    {
        if (EffectObject == null) return;

        CacheParticleSystem();

        if (_particleSystem != null)
        {
            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        EffectObject.SetActive(false);
    }

    private void CacheParticleSystem()
    {
        if (_isCached) return;

        _particleSystem = EffectObject.GetComponent<ParticleSystem>();
        _isCached = true;
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

    private Dictionary<PlayerEffectType, PlayerEffectHandler> _effects;

    private void Awake()
    {
        _effects = new Dictionary<PlayerEffectType, PlayerEffectHandler>
        {
            [PlayerEffectType.Skill] = _skillEffect,
            [PlayerEffectType.Heal] = _healEffect,
            [PlayerEffectType.SkillUp] = _skillBoostEffect
        };
    }
    private void OnEnable()
    {
        StopAllEffects();
    }

    public void StopAllEffects()
    {
        StopAllSlashes();
        if (_effects == null) return;

        foreach (var handler in _effects.Values)
        {
            handler?.Stop();
        }
    }

    public void PlayEffect(PlayerEffectType type)
    {
        if (_effects == null) return;

        if (_effects.TryGetValue(type, out var handler))
        {
            handler?.Play();
        }
    }

    public void StopEffect(PlayerEffectType type)
    {
        if (_effects == null) return;

        if (_effects.TryGetValue(type, out var handler))
        {
            handler?.Stop();
        }
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
}
