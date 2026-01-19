using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignetteEffect : MonoBehaviour
{
    [SerializeField] private Volume _volume;
    [SerializeField] private PlayerStat _playerStat;
    [SerializeField] private PlayerSound _playerSound;
    [SerializeField, Range(0f, 1f)] private float _healthThreshold = 0.2f;
    [SerializeField] private float _vignetteIntensity = 0.4f;
    [SerializeField] private float _pulseDuration = 0.8f;

    private Vignette _vignette;
    private Tween _pulseTween;
    private bool _isLowHealth;

    private void Start()
    {
        if (_volume != null)
        {
            _volume.profile.TryGet(out _vignette);
            _volume.gameObject.SetActive(false);
        }

        if (_playerStat != null)
        {
            _playerStat.OnHealthChanged += HandleHealthChanged;
        }
    }

    private void OnDestroy()
    {
        _pulseTween?.Kill();

        if (_playerStat != null)
        {
            _playerStat.OnHealthChanged -= HandleHealthChanged;
        }
    }

    private void HandleHealthChanged(float current, float max)
    {
        if (_volume == null) return;

        float healthPercent = current / max;

        if (healthPercent <= _healthThreshold)
        {
            if (!_isLowHealth)
            {
                _isLowHealth = true;
                StartPulse();
            }
        }
        else
        {
            if (_isLowHealth)
            {
                _isLowHealth = false;
                StopPulse();
            }
        }
    }

    private void StartPulse()
    {
        _pulseTween?.Kill();
        _volume.gameObject.SetActive(true);

        if (_vignette == null) return;

        PlayHeartBeat();

        _pulseTween = DOTween.To(
            () => _vignette.intensity.value,
            x => _vignette.intensity.value = x,
            _vignetteIntensity,
            _pulseDuration
        )
        .SetEase(Ease.InOutSine)
        .SetLoops(-1, LoopType.Yoyo)
        .OnStepComplete(PlayHeartBeat);
    }

    private void PlayHeartBeat()
    {
        _playerSound?.PlayHeartBeat();
    }

    private void StopPulse()
    {
        _pulseTween?.Kill();
        _volume.gameObject.SetActive(false);
    }
}
