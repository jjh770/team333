using UnityEngine;

public class TraceMoveComponent : MoveComponent
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 4f;
    [SerializeField] private float _updateInterval = 0.2f;
    [SerializeField] private float _stoppingDistance = 2f;

    private float _updateTimer;

    protected override void Start()
    {
        base.Start();
        _agent.speed = _moveSpeed;
        _agent.stoppingDistance = _stoppingDistance;
    }

    private void Update()
    {
        if (_player == null) return;

        Trace();
    }

    private void Trace()
    {
        _updateTimer -= Time.deltaTime;
        if (_updateTimer <= 0f)
        {
            _agent.SetDestination(_player.position);
            _updateTimer = _updateInterval;
        }
    }
}
