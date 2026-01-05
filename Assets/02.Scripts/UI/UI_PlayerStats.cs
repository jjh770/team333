using UnityEngine;
using TMPro;

public class UI_PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;

    [SerializeField] private TextMeshProUGUI _healthTextUI;
    [SerializeField] private TextMeshProUGUI _speedTextUI;
    [SerializeField] private TextMeshProUGUI _jumpForceTextUI;
    
    private void Update()
    {
        _healthTextUI.text = $"Health: {_playerStats.Health.CurrentValue} / {_playerStats.Health.MaxValue}";
        _speedTextUI.text = $"Speed: {_playerStats.Speed.Value}";
        _jumpForceTextUI.text = $"JumpForce: {_playerStats.JumpForce.Value}";
    }
}
