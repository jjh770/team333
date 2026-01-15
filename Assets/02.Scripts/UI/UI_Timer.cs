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
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100f) % 100f);

        return $"{minutes:00}:{seconds:00}.{milliseconds:00}";
    }
}
