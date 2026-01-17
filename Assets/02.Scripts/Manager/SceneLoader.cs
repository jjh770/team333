using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string _lobbySceneName = "MainLobby";
    [SerializeField] private string _gameSceneName = "GameScene";
    [SerializeField] private string _endSceneName = "EndScene";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        UI_PauseMenu.OnRestartRequested += ReloadCurrentScene;
        UI_PauseMenu.OnLobbyRequested += LoadLobby;
    }

    private void OnDisable()
    {
        UI_PauseMenu.OnRestartRequested -= ReloadCurrentScene;
        UI_PauseMenu.OnLobbyRequested -= LoadLobby;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void LoadLobby()
    {
        LoadSceneByName(_lobbySceneName);
    }

    public void LoadGame()
    {
        LoadSceneByName(_gameSceneName);
    }

    public void LoadEndScene()
    {
        LoadSceneByName(_endSceneName);
    }

    public void ReloadCurrentScene()
    {
        LoadSceneByName(SceneManager.GetActiveScene().name);
    }

    private void LoadSceneByName(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
