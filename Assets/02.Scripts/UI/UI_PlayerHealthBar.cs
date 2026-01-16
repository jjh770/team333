using MoreMountains.Tools;
using UnityEngine;

public class UI_PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private PlayerStat _stat;
    [SerializeField] private MMProgressBar _healthBar;

    private void OnEnable()
    {
        _stat.OnHealthChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        _stat.OnHealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float current, float max)
    {
        _healthBar.UpdateBar(current, 0f, max);
    }
}
