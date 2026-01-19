using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_EndScene : MonoBehaviour
{
    [Header("Result Display")]
    [SerializeField] private TMP_Text _clearTimeText;
    [SerializeField] private TMP_Text _rankText;

    [Header("Name Input")]
    [SerializeField] private TMP_InputField _nameInputField;
    [SerializeField] private Button _submitButton;
    [SerializeField] private GameObject _inputPanel;

    [Header("Leaderboard")]
    [SerializeField] private GameObject _leaderboardPanel;
    [SerializeField] private Transform _leaderboardContainer;
    [SerializeField] private GameObject _entryPrefab;

    [Header("Buttons")]
    [SerializeField] private GameObject _buttonPanel;
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _mainMenuButton;

    [Header("Settings")]
    [SerializeField] private string _gameSceneName = "yj";
    [SerializeField] private string _mainMenuSceneName = "MainMenu";
    [SerializeField] private int _maxDisplayEntries = 5;

    [Header("Animation")]
    [SerializeField] private UI_EndSceneAnimator _animator;

    private float _clearTime;
    private bool _hasSubmitted;

    private void Start()
    {
        _clearTime = GameTimeManager.LastElapsedTime;

        SetupUI();
        SetupButtons();
        UpdateLeaderboardDisplay();
    }

    private void SetupUI()
    {
        if (_clearTimeText != null)
        {
            _clearTimeText.text = FormatTime(_clearTime);
        }

        if (_rankText != null)
        {
            int rank = LeaderboardManager.Instance != null
                ? LeaderboardManager.Instance.GetRank(_clearTime)
                : 1;
            _rankText.text = $"{rank}";
        }
    }

    private void SetupButtons()
    {
        if (_submitButton != null)
        {
            _submitButton.onClick.AddListener(OnSubmitClicked);
        }

        if (_retryButton != null)
        {
            _retryButton.onClick.AddListener(OnRetryClicked);
        }

        if (_mainMenuButton != null)
        {
            _mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }
    }

    private void OnSubmitClicked()
    {
        if (_hasSubmitted) return;

        string playerName = _nameInputField != null ? _nameInputField.text.Trim() : "";

        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "Player";
        }

        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.AddEntry(playerName, _clearTime);
        }

        _hasSubmitted = true;

        UpdateLeaderboardDisplay();

        if (_animator != null)
        {
            _animator.HidePanel(UI_EndSceneAnimator.InputPanelName);
            _animator.PlayPanel(UI_EndSceneAnimator.LeaderboardPanelName);
            _animator.PlayPanel(UI_EndSceneAnimator.ButtonPanelName);
        }
    }

    private void UpdateLeaderboardDisplay()
    {
        if (_leaderboardContainer == null || _entryPrefab == null) return;
        if (LeaderboardManager.Instance == null) return;

        foreach (Transform child in _leaderboardContainer)
        {
            Destroy(child.gameObject);
        }

        var entries = LeaderboardManager.Instance.Entries;
        int displayCount = Mathf.Min(entries.Count, _maxDisplayEntries);

        for (int i = 0; i < displayCount; i++)
        {
            var entry = entries[i];
            var entryObject = Instantiate(_entryPrefab, _leaderboardContainer);

            var entryUI = entryObject.GetComponent<UI_LeaderboardEntry>();
            if (entryUI != null)
            {
                entryUI.Setup(i + 1, entry.Name, entry.Time);
            }
        }
    }

    private void OnRetryClicked()
    {
        SceneManager.LoadScene(_gameSceneName);
    }

    private void OnMainMenuClicked()
    {
        SceneManager.LoadScene(_mainMenuSceneName);
    }

    private string FormatTime(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        return $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}.{timeSpan.Milliseconds / 10:00}";
    }
}
