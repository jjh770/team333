using UnityEngine;
using UnityEngine.UI;

public class UI_LobbyMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _leaderboardButton;
    [SerializeField] private Button _quitButton;
    [Header("Leaderboard")]
    [SerializeField] private UI_LobbyLeaderboard _lobbyLeaderboard;
    private void Start()
    {
        _startButton.onClick.AddListener(OnStartClicked);
        _leaderboardButton.onClick.AddListener(OnLeaderboardClicked);
        _quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnDestroy()
    {
        _startButton.onClick.RemoveListener(OnStartClicked);
        _leaderboardButton.onClick.RemoveListener(OnLeaderboardClicked);
        _quitButton.onClick.RemoveListener(OnQuitClicked);
    }

    private void OnStartClicked()
    {
        SceneLoader.Instance.LoadGame();
    }

    private void OnLeaderboardClicked()
    {
        if (_lobbyLeaderboard != null)
        {
            _lobbyLeaderboard.Show();
        }
    }

    private void OnQuitClicked()
    {
        SceneLoader.Instance.QuitGame();
    }
}
