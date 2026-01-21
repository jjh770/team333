using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_SpeedGauge : MonoBehaviour
{
    private enum SpeedState { Low, Mid, High }

    [SerializeField] private FloraSpeedGaugeController _controller;
    [SerializeField] private Slider _speedGaugeSlider;

    [Header("Thresholds")]
    [SerializeField] private float _highThreshold = 0.66f;
    [SerializeField] private float _lowThreshold = 0.33f;
    [SerializeField] private float _gaugeDuration = 0.3f;
    [SerializeField] private Ease _gaugeEase;



    [Header("Icons")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private Sprite _iconLow;
    [SerializeField] private Sprite _iconMid;
    [SerializeField] private Sprite _iconHigh;

    private SpeedState? _currentState;
    private RectTransform _iconRectTransform;
    private RectTransform _sliderRectTransform;
    private Tweener _sliderTween;

    private void Awake()
    {
        _iconRectTransform = _iconImage.GetComponent<RectTransform>();
        _sliderRectTransform = _speedGaugeSlider.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        _controller.GaugeChanged += UpdateUI;
    }

    private void OnDisable()
    {
        _controller.GaugeChanged -= UpdateUI;
        _sliderTween?.Kill();
    }

    private void UpdateUI(float current, float max)
    {
        float targetValue = max > 0f ? current / max : 0f;

        _sliderTween?.Kill();
        _sliderTween = _speedGaugeSlider.DOValue(targetValue, _gaugeDuration)
            .SetEase(_gaugeEase)
            .OnUpdate(() =>
            {
                UpdateIconPosition(_speedGaugeSlider.value);
                UpdateIconSprite(_speedGaugeSlider.value);
            });
    }

    private void UpdateIconPosition(float value)
    {
        float sliderWidth = _sliderRectTransform.rect.width;
        float xPos = sliderWidth * value;
        _iconRectTransform.anchoredPosition = new Vector2(xPos, _iconRectTransform.anchoredPosition.y);
    }

    private void UpdateIconSprite(float value)
    {
        var newState = value >= _highThreshold ? SpeedState.High
                     : value >= _lowThreshold ? SpeedState.Mid
                     : SpeedState.Low;

        if (_currentState != newState)
        {
            _currentState = newState;
            _iconImage.sprite = newState switch
            {
                SpeedState.High => _iconHigh,
                SpeedState.Mid => _iconMid,
                _ => _iconLow
            };
        }
    }
}
