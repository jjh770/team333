using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class FloraMovement : MonoBehaviour
{
    private NavMeshAgent _agent;
    private FloraStats _stats;
    private FloraAnimationController _animationController;
    private IFloraPath _path;

    private IFloraState _currentState;

    public FloraIdleState IdleState { get; private set; }
    public FloraMoveState MoveState { get; private set; }
    public FloraWaitState WaitState { get; private set; }

    public FloraAnimationController AnimationController => _animationController;
    public IFloraPath Path => _path;
    public float CurrentSpeed => _agent.speed;
    public bool ShouldWait => _path.ShouldWait;

    public void Awake()
    {
        Initialize();
    }
    
    private void Initialize()
    {
        _stats = GetComponent<FloraStats>();
        _path = GetComponent<IFloraPath>();
        _agent = GetComponent<NavMeshAgent>();
        _animationController = GetComponentInChildren<FloraAnimationController>();

        if (_stats != null)
        {
            _stats.SpeedChanged += OnSpeedChanged;
            _agent.speed = _stats.CurrentSpeed;
            _animationController?.Initialize(_stats.MaxSpeed);
        }

        IdleState = new FloraIdleState(this);
        MoveState = new FloraMoveState(this);
        WaitState = new FloraWaitState(this);

        ChangeState(MoveState);
    }

    public void Resume()
    {
        if (_currentState == WaitState)
        {
            _path.MoveNext();
            ChangeState(MoveState);
        }
    }

    private void OnDisable()
    {
        if (_stats != null)
            _stats.SpeedChanged -= OnSpeedChanged;
    }

    private void Update()
    {
        _currentState?.Update();
    }

    public void ChangeState(IFloraState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();
    }

    public bool HasNextDestination()
    {
        return !_path.IsFinished;
    }

    public bool HasReachedDestination()
    {
        return !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance;
    }

    public void SetNextDestination()
    {
        if (_path.IsFinished)
            return;

        Vector3 target = _path.GetCurrentPoint();
        _agent.SetDestination(target);
    }

    public void AdvancePath()
    {
        if (_path.IsFinished)
            return;

        _path.MoveNext();
    }

    private void OnSpeedChanged(float current)
    {
        _agent.speed = current;
        _animationController?.SetMovementSpeed(current);
    }
}
