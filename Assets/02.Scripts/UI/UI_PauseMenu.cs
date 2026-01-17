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

    private void Start()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnStateChanged += HandleStateChanged;
        }

        _resumeButton.onClick.AddListener(OnResumeClicked);
        _restartButton.onClick.AddListener(OnRestartClicked);
        _lobbyButton.onClick.AddListener(OnLobbyClicked);

        _pausePanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnStateChanged -= HandleStateChanged;
        }

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
        GameStateManager.Instance.ResumeGame();
    }

    private void OnRestartClicked()
    {
        GameStateManager.Instance.RestartScene();
    }

    private void OnLobbyClicked()
    {
        GameStateManager.Instance.GoToLobby();
    }
}
