using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_DeathMenu : MonoBehaviour
{
    [Header("UI Panel")]
    [SerializeField] private GameObject _deathPanel;

    [Header("Buttons")]
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _lobbyButton;

    public static event Action OnRestartRequested;
    public static event Action OnLobbyRequested;

    private void Start()
    {
        GameStateManager.OnDeathUIReady += ShowDeathPanel;
        GameStateManager.OnGameStateChanged += HandleStateChanged;

        _restartButton.onClick.AddListener(OnRestartClicked);
        _lobbyButton.onClick.AddListener(OnLobbyClicked);

        _deathPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        GameStateManager.OnDeathUIReady -= ShowDeathPanel;
        GameStateManager.OnGameStateChanged -= HandleStateChanged;

        _restartButton.onClick.RemoveListener(OnRestartClicked);
        _lobbyButton.onClick.RemoveListener(OnLobbyClicked);
    }

    private void ShowDeathPanel()
    {
        _deathPanel.SetActive(true);
    }

    private void HandleStateChanged(GameState oldState, GameState newState)
    {
        if (oldState == GameState.Dead && newState != GameState.Dead)
        {
            _deathPanel.SetActive(false);
        }
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
