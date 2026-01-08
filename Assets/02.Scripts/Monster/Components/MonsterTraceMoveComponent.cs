using UnityEngine;

[RequireComponent(typeof(MonsterStat))]
public class MonsterTraceMoveComponent : MonsterMoveComponent
{
    [Header("Movement")]
    [SerializeField] private float _updateInterval = 0.2f;

    private MonsterStat _stat;
    private float _updateTimer;

    public bool IsMoving { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        _stat = GetComponent<MonsterStat>();
    }

    protected override void Start()
    {
        base.Start();
        _agent.stoppingDistance = _stat.AttackDistance.Value;
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

    private void Update()
    {
        UpdateMoveState();
        UpdateTraceTarget();
    }

    private void UpdateMoveState()
    {
        bool isMoving = _agent.velocity.sqrMagnitude > _velocityThreshold * _velocityThreshold;

        if (IsMoving != isMoving)
        {
            IsMoving = isMoving;
            var state = IsMoving ? MonsterState.Move : MonsterState.Idle;
            _monsterController.ChangeState(state);
        }
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
