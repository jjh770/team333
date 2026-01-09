using UnityEngine;

public class MonsterTraceMoveComponent : MonsterMoveComponent
{
    [Header("Movement")]
    [SerializeField] private float _updateInterval = 0.2f;
    [SerializeField] private float _stoppingDistance = 1.8f;

    private float _updateTimer;

    protected override void Awake()
    {
        base.Awake();
        _agent.stoppingDistance = _stoppingDistance;
    }

    public void SetSpeed(float value)
    {
        _agent.speed = value;
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
}
