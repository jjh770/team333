using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_PauseMenu : MonoBehaviour
{
    [Header("UI Panel")]
    [SerializeField] private GameObject _pausePanel;

    [Header("Buttons")]
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _lobbyButton;

    public static event Action OnResumeRequested;
    public static event Action OnRestartRequested;
    public static event Action OnLobbyRequested;

    private void Start()
    {
        GameStateManager.OnGameStateChanged += HandleStateChanged;

        _resumeButton.onClick.AddListener(OnResumeClicked);
        _restartButton.onClick.AddListener(OnRestartClicked);
        _lobbyButton.onClick.AddListener(OnLobbyClicked);

        _pausePanel.SetActive(false);
    }

    private void OnDestroy()
    {
        GameStateManager.OnGameStateChanged -= HandleStateChanged;

        _resumeButton.onClick.RemoveListener(OnResumeClicked);
        _restartButton.onClick.RemoveListener(OnRestartClicked);
        _lobbyButton.onClick.RemoveListener(OnLobbyClicked);
    }

    private void HandleStateChanged(GameState oldState, GameState newState)
    {
        _pausePanel.SetActive(newState == GameState.Paused);
    }

    private void OnResumeClicked()
    {
        OnResumeRequested?.Invoke();
    }

    private void OnRestartClicked()
    {
        OnRestartRequested?.Invoke();
    }

    private void OnLobbyClicked()
    {
        OnLobbyRequested?.Invoke();
    }
}
