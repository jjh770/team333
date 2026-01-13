using System;
using UnityEngine;

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

    private GameState _currentState;
    private IFloraPath _floraPath;
    
    public GameState CurrentState => _currentState;
    public bool IsPlaying => _currentState == GameState.Playing;

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
                _cameraController.StartPlaying();
                break;

            case GameState.Outro:
                _cameraController.StartOutro();
                break;

            case GameState.Paused:
                //Time.timeScale = 0f;
                break;
        }
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
        // SceneManager.LoadScene("EndScene");
    }
}