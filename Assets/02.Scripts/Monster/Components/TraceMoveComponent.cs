using UnityEngine;

[RequireComponent(typeof(MonsterStat))]
public class TraceMoveComponent : MoveComponent
{
    [Header("Movement")]
    [SerializeField] private float _updateInterval = 0.2f;
    [SerializeField] private float _stoppingDistance = 2f;

    private MonsterStat _stat;
    private float _updateTimer;

    protected override void Awake()
    {
        base.Awake();
        _stat = GetComponent<MonsterStat>();
    }

    protected override void Start()
    {
        base.Start();
        _agent.stoppingDistance = _stoppingDistance;
    }

    private void OnEnable()
    {
        _stat.OnMoveSpeedChanged += OnMoveSpeedChanged;
    }

    private void OnDisable()
    {
        _stat.OnMoveSpeedChanged -= OnMoveSpeedChanged;
    }

    private void OnMoveSpeedChanged(float value)
    {
        _agent.speed = value;
    }

    protected override void Update()
    {
        base.Update();
        UpdateTraceTarget();
    }

    private void UpdateTraceTarget()
    {
        if (_player == null) return;

        _updateTimer -= Time.deltaTime;
        if (_updateTimer <= 0f)
        {
            _agent.SetDestination(_player.position);
            _updateTimer = _updateInterval;
        }
    }
}
