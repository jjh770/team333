using UnityEngine;
using UnityEngine.UI;

public class UI_SpeedGauge : MonoBehaviour
{
    private const float LOW_THRESHOLD = 0.3f;
    private const float HIGH_THRESHOLD = 0.7f;

    [SerializeField] private FloraSpeedGaugeController _controller;
    [SerializeField] private Slider _speedGaugeSlider;

    [Header("Icon Settings")]
    [SerializeField] private RectTransform _iconTransform;
    [SerializeField] private Image _iconImage;

    [Header("Icon Sprites")]
    [SerializeField] private Sprite _iconLow;
    [SerializeField] private Sprite _iconMid;
    [SerializeField] private Sprite _iconHigh;

    private int _currentState = -1;

    private void OnEnable()
    {
        _controller.GaugeChanged += UpdateUI;
    }

    private void OnDisable()
    {
        _controller.GaugeChanged -= UpdateUI;
    }

    private void UpdateUI(float current, float max)
    {
        float value = max > 0f ? current / max : 0f;
        _speedGaugeSlider.value = value;

        if (_iconTransform == null) return;

        // Slider value로 anchor 직접 설정
        _iconTransform.anchorMin = new Vector2(value, 0.5f);
        _iconTransform.anchorMax = new Vector2(value, 0.5f);

        // 상태 변경 시에만 스프라이트 교체
        int newState = value >= HIGH_THRESHOLD ? 2 : (value >= LOW_THRESHOLD ? 1 : 0);
        if (_currentState != newState)
        {
            _currentState = newState;
            _iconImage.sprite = newState switch
            {
                2 => _iconHigh,
                1 => _iconMid,
                _ => _iconLow
            };
        }
    }
}
