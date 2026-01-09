using UnityEngine;
using UnityEngine.UI;

public class UI_SpeedGauge : MonoBehaviour
{
    [SerializeField] private FloraSpeedGaugeController _controller;
    [SerializeField] private Slider _speedGaugeSlider;

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
        _speedGaugeSlider.value = current / max;
    }
}
