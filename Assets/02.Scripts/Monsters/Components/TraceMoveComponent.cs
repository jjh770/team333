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
        agent.speed = _moveSpeed;
        agent.stoppingDistance = _offset;
    }

    void Update()
    {
        TryTrace();
    }

    private void TryTrace()
    {
        if (player == null) return;

        _updateTimer -= Time.deltaTime;
        if (_updateTimer <= 0f)
        {
            Trace();
            _updateTimer = _updateInterval;
        }
    }

    private void Trace()
    {
        agent.SetDestination(player.position);
    }
}
