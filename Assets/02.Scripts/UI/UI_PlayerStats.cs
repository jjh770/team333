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
        _playerStats.HealthChanged += UpdateHealthText;
        _playerStats.SpeedChanged += UpdateSpeedText;
        _playerStats.JumpForceChanged += UpdateJumpForceText;
    }

    private void OnDisable()
    {
        _playerStats.HealthChanged -= UpdateHealthText;
        _playerStats.SpeedChanged -= UpdateSpeedText;
        _playerStats.JumpForceChanged -= UpdateJumpForceText;
    }

    private void UpdateHealthText(float current, float max)
    {
        _healthTextUI.text = $"Health: {current:0} / {max:0}";
    }

    private void UpdateSpeedText(float current, float max)
    {
        _speedTextUI.text = $"Speed: {current:0} / {max:0}";
    }

    private void UpdateJumpForceText(float value)
    {
        _jumpForceTextUI.text = $"JumpForce: {value:0}";
    }
}
