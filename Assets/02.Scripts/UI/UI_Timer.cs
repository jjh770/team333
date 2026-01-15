using System;
using UnityEngine;
using TMPro;

public class UI_Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private GameTimeManager _gameTimeManager;

    private void Update()
    {
        if (_gameTimeManager == null) return;
        if (!_gameTimeManager.HasStarted) return;

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (_timerText != null)
        {
            _timerText.text = FormatTime(_gameTimeManager.ElapsedTime);
        }
    }

    private string FormatTime(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        
        return $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}.{timeSpan.Milliseconds / 10:00}";
    }
}
