using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Start,
    Intro,     
    Playing,   
    Outro,      
    Paused     
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private FloraInteraction _floraInteraction;
    [SerializeField] private CameraController _cameraController;
    
    [Header("Settings")]
    [SerializeField] private string _endSceneName = "EndScene";
    [SerializeField] private string _lobbySceneName = "MainLobby";

    private GameState _currentState;
    private GameState _previousState;
    private IFloraPath _floraPath;

    public GameState CurrentState => _currentState;
    public bool IsPlaying => _currentState == GameState.Playing;
    public bool IsPaused => _currentState == GameState.Paused;

    public event Action<GameState, GameState> OnStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        _currentState = GameState.Start;
    }

    private void Start()
    {
        if (_floraInteraction != null)
        {
            _floraPath = _floraInteraction.FloraPath;
            
            if (_floraPath != null)
            {
                _floraPath.OnPathCompleted += HandlePathCompleted;
            }
        }
        
        if (_cameraController != null)
        {
            _cameraController.OnIntroComplete += HandleIntroComplete;
            _cameraController.OnOutroComplete += HandleOutroComplete;
        }
      
        ChangeState(GameState.Intro);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        if (_floraPath != null)
        {
            _floraPath.OnPathCompleted -= HandlePathCompleted;
        }

        if (_cameraController != null)
        {
            _cameraController.OnIntroComplete -= HandleIntroComplete;
            _cameraController.OnOutroComplete -= HandleOutroComplete;
        }
    }

    public void ChangeState(GameState newState)
    {
        if (_currentState == newState)
        {
            return;
        }
        
        GameState oldState = _currentState;
        _currentState = newState;

        Debug.Log($"Game State: {oldState} → {newState}");

        OnStateChanged?.Invoke(oldState, newState);

        HandleStateEnter(newState);
    }

    private void HandleStateEnter(GameState state)
    {
        switch (state)
        {
            case GameState.Intro:
                _cameraController.StartIntro();
                break;

            case GameState.Playing:
                Time.timeScale = 1f;
                _cameraController.StartPlaying();
                break;

            case GameState.Outro:
                _cameraController.StartOutro();
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                break;
        }
    }

    public void TogglePause()
    {
        if (_currentState == GameState.Paused)
        {
            ResumeGame();
        }
        else if (_currentState == GameState.Playing)
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (_currentState != GameState.Playing) return;

        _previousState = _currentState;
        ChangeState(GameState.Paused);
    }

    public void ResumeGame()
    {
        if (_currentState != GameState.Paused) return;

        ChangeState(_previousState);
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToLobby()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(_lobbySceneName);
    }

    private void HandlePathCompleted()
    {
        if (_currentState == GameState.Playing)
        {
            ChangeState(GameState.Outro);
        }
    }

    private void HandleIntroComplete()
    {
        ChangeState(GameState.Playing);
    }

    private void HandleOutroComplete()
    {
        Debug.Log("HandleOutroComplete called");
        SceneManager.LoadScene(_endSceneName);
    }
}