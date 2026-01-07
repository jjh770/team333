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

    public FloraAnimationController AnimationController => _animationController;
    public IFloraPath Path => _path;
    public float CurrentSpeed => _agent.speed;

    public void Initialize(FloraStats stats, IFloraPath path, FloraAnimationController animationController)
    {
        _stats = stats;
        _path = path;
        _agent = GetComponent<NavMeshAgent>();
        _animationController = animationController;

        if (_stats != null)
        {
            _stats.SpeedChanged += OnSpeedChanged;
            _agent.speed = _stats.CurrentSpeed;
            _animationController?.Initialize(_stats.MaxSpeed);
        }

        IdleState = new FloraIdleState(this);
        MoveState = new FloraMoveState(this);

        ChangeState(MoveState);
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
        _path.MoveNext();
    }

    private void OnSpeedChanged(float current)
    {
        _agent.speed = current;
        _animationController?.SetMovementSpeed(current);
    }
}
