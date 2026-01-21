using GameUI.Animations;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_PauseMenu : MonoBehaviour
{
    [Header("UI Panel")]
    [SerializeField] private GameObject _pausePanel;

    [SerializeField] private UIFadeAnimation _backgroundFade;
    [SerializeField] private UIElementDelayAnimation _delayedPanelAnimation;

    [Header("Buttons")]
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _lobbyButton;

    public static event Action OnResumeRequested;
    public static event Action OnRestartRequested;
    public static event Action OnLobbyRequested;

    private void Awake()
    {
        _pausePanel.SetActive(false);
        _backgroundFade.SetToHidden();
        _delayedPanelAnimation.SetToHidden();
    }

    private void Start()
    {
        GameStateManager.OnGameStateChanged += HandleStateChanged;

        _resumeButton.onClick.AddListener(OnResumeClicked);
        _restartButton.onClick.AddListener(OnRestartClicked);
        _lobbyButton.onClick.AddListener(OnLobbyClicked);
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
        PanelAnimation(newState == GameState.Paused);
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

    private void PanelAnimation(bool isActive)
    {
        if (isActive)
        {
            _pausePanel.SetActive(true);
            _backgroundFade.AnimateToVisible();
            _delayedPanelAnimation.AnimateToVisible();
        }
        else
        {
            _backgroundFade.AnimateToHidden();
            _delayedPanelAnimation.AnimateToHidden(() =>
            {
                if (!GameStateManager.Instance.IsPaused)
                {
                    _pausePanel.SetActive(false);
                }
            });
        }
    }
}
