using MoreMountains.Tools;
using UnityEngine;

public class UI_PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private PlayerStat _stat;
    [SerializeField] private MMProgressBar _healthBar;

    private void Start()
    {
        _stat.OnHealthChanged += OnHealthChanged;
    }

    private void OnDestroy()
    {
        _stat.OnHealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float current, float max)
    {
        _healthBar.UpdateBar(current, 0f, max);
    }
}
