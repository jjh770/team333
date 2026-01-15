using System;
using UnityEngine;

public class GameTimeManager : MonoBehaviour
{
    public static GameTimeManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private FloraMovement _floraMovement;
    [SerializeField] private SplineWaypointPath _floraPath;
    private float _startTime;
    private float _endTime;
    private bool _isRunning;
    private bool _hasStarted;

    public float ElapsedTime => _isRunning ? Time.time - _startTime : _endTime - _startTime;
    public bool IsRunning => _isRunning;
    public bool HasStarted => _hasStarted;

    public event Action OnTimerStarted;
    public event Action<float> OnTimerStopped;

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
        if (_floraMovement != null)
        {
            _floraMovement.OnResumed += HandleFirstResume;
        }

        if (_floraPath != null)
        {
            _floraPath.OnPathCompleted += HandlePathCompleted;
        }
    }

    private void OnDestroy()
    {
        if (_floraMovement != null)
        {
            _floraMovement.OnResumed -= HandleFirstResume;
        }

        if (_floraPath != null)
        {
            _floraPath.OnPathCompleted -= HandlePathCompleted;
        }
    }

    private void HandleFirstResume()
    {
        if (!_hasStarted)
        {
            StartTimer();
        }
    }

    private void HandlePathCompleted()
    {
        if (_isRunning)
        {
            StopTimer();
        }
    }

    public void StartTimer()
    {
        if (_isRunning) return;

        _startTime = Time.time;
        _isRunning = true;
        _hasStarted = true;

        OnTimerStarted?.Invoke();
    }

    public void StopTimer()
    {
        if (!_isRunning) return;

        _endTime = Time.time;
        _isRunning = false;

        Debug.Log($"[GameTimeManager] Timer stopped - Elapsed: {ElapsedTime:F2}s");
        OnTimerStopped?.Invoke(ElapsedTime);
    }
}
