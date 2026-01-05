using UnityEngine;
using TMPro;

public class UI_PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;

    [SerializeField] private TextMeshProUGUI _healthTextUI;
    [SerializeField] private TextMeshProUGUI _speedTextUI;
    [SerializeField] private TextMeshProUGUI _jumpForceTextUI;

    private void OnEnable()
    {
        _playerStats.Health.OnValueChanged += UpdateHealthText;
        _playerStats.Speed.OnValueChanged += UpdateSpeedText;
        _playerStats.JumpForce.OnValueChanged += UpdateJumpForceText;
    }

    private void OnDisable()
    {
        _playerStats.Health.OnValueChanged -= UpdateHealthText;
        _playerStats.Speed.OnValueChanged -= UpdateSpeedText;
        _playerStats.JumpForce.OnValueChanged -= UpdateJumpForceText;
    }

    private void UpdateHealthText(float currentValue, float maxValue)
    {
        _healthTextUI.text = $"Health: {currentValue:0} / {maxValue:0}";
    }

    private void UpdateSpeedText(float currentValue, float maxValue)
    {
        _speedTextUI.text = $"Speed: {currentValue:0} / {maxValue:0}";
    }

    private void UpdateJumpForceText(float value)
    {
        _jumpForceTextUI.text = $"JumpForce: {value:0}";
    }
}
