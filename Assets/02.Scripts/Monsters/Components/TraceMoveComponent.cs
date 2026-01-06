using UnityEngine;

public class TraceMoveComponent : MoveComponent
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 4f;
    [SerializeField] private float _updateInterval = 0.2f;
    [SerializeField] private float _offset = 0.5f;

    private float _updateTimer;

    protected override void Start()
    {
        base.Start();
        _agent.speed = _moveSpeed;
        _agent.stoppingDistance = _offset;
    }

    private void Update()
    {
        TryTrace();
    }

    private void TryTrace()
    {
        if (_player == null) return;

        _updateTimer -= Time.deltaTime;
        if (_updateTimer <= 0f)
        {
            Trace();
            _updateTimer = _updateInterval;
        }
    }

    private void Trace()
    {
        _agent.SetDestination(_player.position);
    }
}
