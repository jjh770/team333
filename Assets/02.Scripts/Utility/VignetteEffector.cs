using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Vignette 효과를 제어하는 유틸리티 컴포넌트
/// </summary>
public class VignetteEffector : MonoBehaviour
{
    [Header("Pulse Settings")]
    [SerializeField] private float _pulseMinIntensity = 0.3f;
    [SerializeField] private float _pulseMaxIntensity = 0.6f;
    [SerializeField] private float _pulseDuration = 0.4f;
    [SerializeField] private int _pulseCount = 4;

    private Volume _volume;
    private Vignette _vignette;
    private Tween _currentTween;

    public bool IsAvailable => _vignette != null;

    private void Awake()
    {
        Initialize();
    }

    private void OnDestroy()
    {
        _currentTween?.Kill();
    }

    public void Initialize()
    {
        if (_volume != null) return;

        _volume = Camera.main?.GetComponent<Volume>();
        if (_volume != null && _volume.profile.TryGet(out Vignette vignette))
        {
            _vignette = vignette;
        }
    }

    /// <summary>
    /// Vignette intensity를 목표 값으로 페이드
    /// </summary>
    public Tween FadeTo(float targetIntensity, float duration, Ease ease = Ease.OutQuad)
    {
        if (_vignette == null) return null;

        _currentTween?.Kill();
        _currentTween = DOTween.To(
            () => _vignette.intensity.value,
            x => _vignette.intensity.value = x,
            targetIntensity,
            duration).SetEase(ease);

        return _currentTween;
    }

    /// <summary>
    /// 심장 박동 느낌의 펄스 효과 시작
    /// </summary>
    public void StartPulse()
    {
        if (_vignette == null) return;

        _currentTween?.Kill();

        _currentTween = DOTween.Sequence()
            .Append(DOTween.To(
                () => _vignette.intensity.value,
                x => _vignette.intensity.value = x,
                _pulseMaxIntensity,
                _pulseDuration * 0.3f).SetEase(Ease.OutQuad))
            .Append(DOTween.To(
                () => _vignette.intensity.value,
                x => _vignette.intensity.value = x,
                _pulseMinIntensity,
                _pulseDuration * 0.7f).SetEase(Ease.OutQuad))
            .SetLoops(_pulseCount, LoopType.Yoyo);
    }

    /// <summary>
    /// 펄스 효과 중지
    /// </summary>
    public void StopPulse()
    {
        _currentTween?.Kill();
        _currentTween = null;
    }

    /// <summary>
    /// 즉시 intensity 설정
    /// </summary>
    public void SetIntensity(float intensity)
    {
        if (_vignette == null) return;
        _vignette.intensity.value = intensity;
    }
}
