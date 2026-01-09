using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterTraceMoveComponent : MonsterMoveComponent
{
    [Header("Movement")]
    [SerializeField] private float _updateInterval = 0.2f;
    [SerializeField] private float _stoppingDistance = 1.8f;

    private NavMeshAgent _agent;
    private float _updateTimer;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.stoppingDistance = _stoppingDistance;
    }

    public override void Enable()
    {
        _agent.enabled = true;
    }

    public override void Disable()
    {
        _agent.enabled = false;
    }

    public override void Stop()
    {
        _agent.isStopped = true;
        _agent.ResetPath();
    }

    public override void Resume()
    {
        _agent.isStopped = false;
    }

    public override void SetSpeed(float speed)
    {
        _agent.speed = speed;
    }

    public override void ResetMove()
    {
        _agent.enabled = true;
        _agent.ResetPath();
        _agent.isStopped = false;
        _updateTimer = 0f;
    }

    public override void UpdateMove()
    {
        UpdateTraceTarget();
        UpdateMoveState();
    }

    private void UpdateTraceTarget()
    {
        if (_target == null) return;

        _updateTimer -= Time.deltaTime;
        if (_updateTimer <= 0f)
        {
            _agent.SetDestination(_target.position);
            _updateTimer = _updateInterval;
        }
    }

    private void UpdateMoveState()
    {
        _isMoving = _agent.velocity.sqrMagnitude > _velocityThreshold * _velocityThreshold;
    }
}
