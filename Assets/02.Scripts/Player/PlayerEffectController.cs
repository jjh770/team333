using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public enum PlayerEffectType
{
    Skill,
    Heal,
    SkillUp,
    DamageUp
}

public enum SlashEffectMode
{
    VFXGraph,
    ParticleSystem
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

[System.Serializable]
public class VFXGraphEffectHandler
{
    public GameObject EffectObject;

    private VisualEffect _visualEffect;
    private bool _isCached;

    public void Play()
    {
        if (EffectObject == null) return;

        EffectObject.SetActive(true);
        CacheVisualEffect();

        if (_visualEffect != null)
        {
            _visualEffect.Stop();
            _visualEffect.Play();
        }
    }

    public void Stop()
    {
        if (EffectObject == null) return;

        CacheVisualEffect();

        if (_visualEffect != null)
        {
            _visualEffect.Stop();
        }

        EffectObject.SetActive(false);
    }

    private void CacheVisualEffect()
    {
        if (_isCached) return;

        _visualEffect = EffectObject.GetComponent<VisualEffect>();
        _isCached = true;
    }
}

public class PlayerEffectController : MonoBehaviour
{
    [Header("Slash Effect Mode")]
    [SerializeField] private SlashEffectMode _slashEffectMode = SlashEffectMode.VFXGraph;

    [Header("Attack Effects - VFX Graph")]
    [SerializeField] private List<VFXGraphEffectHandler> _slashEffectsVFX;

    [Header("Attack Effects - Particle System")]
    [SerializeField] private List<PlayerEffectHandler> _slashEffectsParticle;

    [Header("Skill Effects")]
    [SerializeField] private PlayerEffectHandler _skillEffect;

    [Header("Item Effects")]
    [SerializeField] private PlayerEffectHandler _healEffect;
    [SerializeField] private PlayerEffectHandler _skillBoostEffect;
    [SerializeField] private PlayerEffectHandler _damageUpEffect;

    private Dictionary<PlayerEffectType, PlayerEffectHandler> _effects;

    private void Awake()
    {
        _effects = new Dictionary<PlayerEffectType, PlayerEffectHandler>
        {
            [PlayerEffectType.Skill] = _skillEffect,
            [PlayerEffectType.Heal] = _healEffect,
            [PlayerEffectType.SkillUp] = _skillBoostEffect,
            [PlayerEffectType.DamageUp] = _damageUpEffect
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

        if (_slashEffectMode == SlashEffectMode.VFXGraph)
        {
            if (comboIndex >= 0 && comboIndex < _slashEffectsVFX.Count)
            {
                _slashEffectsVFX[comboIndex]?.Play();
            }
        }
        else
        {
            if (comboIndex >= 0 && comboIndex < _slashEffectsParticle.Count)
            {
                _slashEffectsParticle[comboIndex]?.Play();
            }
        }
    }

    public void StopAllSlashes()
    {
        foreach (var effect in _slashEffectsVFX)
        {
            effect?.Stop();
        }

        foreach (var effect in _slashEffectsParticle)
        {
            effect?.Stop();
        }
    }

    #endregion
}
