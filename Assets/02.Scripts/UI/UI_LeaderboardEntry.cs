using System;
using UnityEngine;
using TMPro;

public class UI_LeaderboardEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text _rankText;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _timeText;

    public void Setup(int rank, string name, float time)
    {
        if (_rankText != null)
        {
            _rankText.text = $"{rank}";
        }

        if (_nameText != null)
        {
            _nameText.text = name;
        }

        if (_timeText != null)
        {
            _timeText.text = FormatTime(time);
        }
    }

    private string FormatTime(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        return $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}.{timeSpan.Milliseconds / 10:00}";
    }
}
