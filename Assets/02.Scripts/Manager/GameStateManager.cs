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
        ChangeState(GameState.Intro);
        
        _floraPath = _floraInteraction.FloraPath;
        if (_floraPath != null)
        {
            Debug.Log($"Flora Path");
            _floraPath.OnPathCompleted += HandlePathCompleted;
        }
    }

    private void OnDestroy()
    {
        if (_floraPath != null)
        {
            _floraPath.OnPathCompleted -= HandlePathCompleted;
        }
    }

    public void ChangeState(GameState newState)
    {
        if (_currentState == newState) return;
        
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
                Time.timeScale = 0f;
                break;
        }
    }

    private void HandlePathCompleted()
    {
        Debug.Log("Path Completed");
        if (_currentState == GameState.Playing)
        {
            ChangeState(GameState.Outro);
        }
    }

    public void OnIntroComplete()
    {
        ChangeState(GameState.Playing);
    }

    public void OnOutroComplete()
    {
        Debug.Log("Outro Complete - Load Next Scene");
        // SceneManager.LoadScene("EndScene");
    }
}