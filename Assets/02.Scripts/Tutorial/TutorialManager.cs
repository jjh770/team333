using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private FloraInteraction _floraInteraction;
    [SerializeField] private PlayerStat _playerStat;

    [Header("Tutorial Steps")]
    [SerializeField] private List<TutorialStepBase> _steps = new List<TutorialStepBase>();

    private int _currentStepIndex = -1;
    private bool _isRunning;
    private bool _isCompleted;

    public bool IsRunning => _isRunning;
    public bool IsCompleted => _isCompleted;
    public ITutorialStep CurrentStep => _currentStepIndex >= 0 && _currentStepIndex < _steps.Count
        ? _steps[_currentStepIndex]
        : null;

    public static event Action<ITutorialStep> OnStepStarted;
    public static event Action<ITutorialStep> OnStepCompleted;
    public static event Action OnTutorialCompleted;
    public static event Action OnTutorialSkipped;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        GameStateManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        GameStateManager.OnGameStateChanged -= HandleGameStateChanged;

        if (CurrentStep != null)
        {
            CurrentStep.OnCompleted -= HandleStepCompleted;
        }
    }

    private void Update()
    {
        if (_isRunning && CurrentStep != null)
        {
            CurrentStep.OnUpdate();
        }
    }

    private void HandleGameStateChanged(GameState oldState, GameState newState)
    {
        if (oldState == GameState.Intro && newState == GameState.Playing)
        {
            StartTutorial();
        }
    }

    public void StartTutorial()
    {
        if (_isRunning || _isCompleted) return;
        if (_steps.Count == 0)
        {
            Debug.LogWarning("TutorialManager: No tutorial steps configured.");
            return;
        }

        Debug.Log("Tutorial Started");
        _isRunning = true;
        _currentStepIndex = -1;

        SetFloraMoveLock(true);
        AdvanceToNextStep();
    }

    public void SkipTutorial()
    {
        if (!_isRunning) return;

        Debug.Log("Tutorial Skipped");

        if (CurrentStep != null)
        {
            CurrentStep.OnCompleted -= HandleStepCompleted;
            CurrentStep.Exit();
        }

        _isRunning = false;
        _isCompleted = true;
        SetFloraMoveLock(false);
        ResetStatsForTutorialEnd();
        OnTutorialSkipped?.Invoke();
    }

    private void AdvanceToNextStep()
    {
        if (CurrentStep != null)
        {
            CurrentStep.OnCompleted -= HandleStepCompleted;
            CurrentStep.Exit();
            OnStepCompleted?.Invoke(CurrentStep);
        }

        _currentStepIndex++;

        if (_currentStepIndex >= _steps.Count)
        {
            CompleteTutorial();
            return;
        }

        var step = _steps[_currentStepIndex];
        step.OnCompleted += HandleStepCompleted;
        step.Enter();

        Debug.Log($"Tutorial Step Started: {step.StepId}");
        OnStepStarted?.Invoke(step);
    }

    private void HandleStepCompleted()
    {
        Debug.Log($"Tutorial Step Completed: {CurrentStep?.StepId}");
        AdvanceToNextStep();
    }

    private void CompleteTutorial()
    {
        Debug.Log("Tutorial Completed");
        _isRunning = false;
        _isCompleted = true;
        SetFloraMoveLock(false);
        ResetStatsForTutorialEnd();
        OnTutorialCompleted?.Invoke();
    }

    private void SetFloraMoveLock(bool isLocked)
    {
        if (_floraInteraction != null)
        {
            _floraInteraction.SetMoveLock(isLocked);
        }
    }

    private void ResetStatsForTutorialEnd()
    {
        if (_floraInteraction != null)
        {
            _floraInteraction.ResetForTutorialEnd();
        }

        if (_playerStat != null)
        {
            _playerStat.FullHeal();
        }
    }
}
