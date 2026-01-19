using UnityEngine;
using UnityEngine.UI;

public class UI_LobbyMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _leaderboardButton;
    [SerializeField] private Button _quitButton;

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
        // TODO: 리더보드 패널 구현 예정
        Debug.Log("Leaderboard - Coming Soon");
    }

    private void OnQuitClicked()
    {
        SceneLoader.Instance.QuitGame();
    }
}
