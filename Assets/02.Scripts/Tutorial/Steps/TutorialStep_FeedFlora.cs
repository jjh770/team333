using System;
using UnityEngine;

public class TutorialStep_FeedFlora : TutorialStepBase
{
    [Header("Clear Condition")]
    [SerializeField] private int _requiredCount = 2;

    [Header("References")]
    [SerializeField] private FloraSpeedGaugeController _floraGaugeController;

    private int _currentCount;

    public int CurrentCount => _currentCount;
    public int RequiredCount => _requiredCount;

    public event Action OnCountChanged;

    protected override void OnEnter()
    {
        _currentCount = 0;

        if (_floraGaugeController != null)
        {
            _floraGaugeController.GaugeChanged += HandleGaugeChanged;
        }
    }

    protected override void OnExit()
    {
        if (_floraGaugeController != null)
        {
            _floraGaugeController.GaugeChanged -= HandleGaugeChanged;
        }
    }

    protected override void CheckCompletion() { }

    private void HandleGaugeChanged(float current, float max)
    {
        _currentCount++;
        OnCountChanged?.Invoke();

        if (_currentCount >= _requiredCount)
        {
            Complete();
        }
    }
}
