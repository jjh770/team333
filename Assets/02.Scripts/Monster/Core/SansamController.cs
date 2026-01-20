using UnityEngine;
using UnityEngine.AI;

public class SansamController : BadMonsterController
{
    private enum SansamBehavior
    {
        FollowFlora,
        AvoidPlayerToFlora
    }

    [Header("Sansam Settings")]
    [SerializeField] private float _playerDetectRange = 3f;
    [SerializeField] private float _playerSafeRange = 5f;
    [SerializeField] private float _fleeSpeedMultiplier = 2f;
    [SerializeField] private float _avoidDistance = 3f;
    [SerializeField] private float _detectionInterval = 0.2f;

    private Transform _floraTransform;
    private Transform _playerTransform;
    private NavMeshAgent _agent;

    private SansamBehavior _behavior = SansamBehavior.FollowFlora;
    private float _detectionTimer;
    private float _baseSpeed;

    private float _agentMovingVelocitySqrThreshold = 0.01f;

    protected override void Awake()
    {
        base.Awake();
        _agent = GetComponent<NavMeshAgent>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        FindTargets();
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
        FindTargets();
        _behavior = SansamBehavior.FollowFlora;
        _baseSpeed = _stat.GetMoveSpeed();

        if (_floraTransform != null)
        {
            _move.SetTarget(_floraTransform);
            _attack.SetTarget(_floraTransform);
            CurrentTarget = _floraTransform;
        }
    }

    private void FindTargets()
    {
        var flora = GameObject.FindGameObjectWithTag("Flora");
        if (flora != null)
        {
            _floraTransform = flora.transform;
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }
    }

    protected override void OnUpdate()
    {
        UpdateDetection();
        UpdateBehavior();
    }

    private void UpdateDetection()
    {
        _detectionTimer -= Time.deltaTime;
        if (_detectionTimer > 0f) return;

        _detectionTimer = _detectionInterval;

        if (_playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

        switch (_behavior)
        {
            case SansamBehavior.FollowFlora:
                if (distanceToPlayer <= _playerDetectRange)
                {
                    SwitchToAvoid();
                }
                break;

            case SansamBehavior.AvoidPlayerToFlora:
                if (distanceToPlayer > _playerSafeRange)
                {
                    SwitchToFollowFlora();
                }
                break;
        }
    }

    private void SwitchToAvoid()
    {
        _behavior = SansamBehavior.AvoidPlayerToFlora;
        _move.SetSpeed(_baseSpeed * _fleeSpeedMultiplier);
    }

    private void SwitchToFollowFlora()
    {
        _behavior = SansamBehavior.FollowFlora;
        _move.SetSpeed(_baseSpeed);

        if (_floraTransform != null)
        {
            _move.SetTarget(_floraTransform);
            CurrentTarget = _floraTransform;
        }
    }

    private void UpdateBehavior()
    {
        switch (_behavior)
        {
            case SansamBehavior.FollowFlora:
                FollowFlora();
                break;

            case SansamBehavior.AvoidPlayerToFlora:
                AvoidPlayerAndGoToFlora();
                break;
        }
    }

    private void FollowFlora()
    {
        if (_floraTransform == null) return;

        _move.UpdateMove();

        if (_attack.TryAttack())
        {
            _move.LookAtTargetImmediate();
        }
    }

    private void AvoidPlayerAndGoToFlora()
    {
        if (_floraTransform == null || _playerTransform == null || _agent == null) return;

        // 산삼 -> 플레이어 벡터
        Vector3 toPlayer = _playerTransform.position - transform.position;
        toPlayer.y = 0f;

        // 산삼 -> 플로라 벡터
        Vector3 toFlora = _floraTransform.position - transform.position;
        toFlora.y = 0f;

        // 플레이어 방향에 수직인 방향 구하기 (즉, 플레이어의 좌우)
        Vector3 perpendicular = Vector3.Cross(toPlayer.normalized, Vector3.up);

        // 왼쪽, 오른쪽 중 어디가 플로라랑 더 가까운지
        float dotRight = Vector3.Dot(perpendicular, toFlora.normalized);
        if (dotRight < 0)
        {
            perpendicular = -perpendicular;
        }

        // 플레이어를 피해 옆으로 이동
        Vector3 avoidPoint = transform.position + perpendicular * _avoidDistance;

        if (NavMesh.SamplePosition(avoidPoint, out NavMeshHit hit, _avoidDistance, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }
    }

    protected override MonsterState GetCurrentState()
    {
        if (_isDead) return MonsterState.Die;
        if (_health.IsDamaged) return MonsterState.Damage;
        if (_attack.IsAttacking) return MonsterState.Attack;

        if (_behavior == SansamBehavior.AvoidPlayerToFlora)
        {
            if (IsAgentMoving())
            {
                return MonsterState.Move;
            }
        }

        if (_move.IsMoving) return MonsterState.Move;
        return MonsterState.Idle;
    }

    private bool IsAgentMoving()
    {
        return _agent != null && _agent.velocity.sqrMagnitude > _agentMovingVelocitySqrThreshold;
    }

    protected override void HandleItemDrop()
    {
        if (Random.value <= _stat.GetDropChance())
        {
            _itemDrop.DropItem(_stat.GetDropItem());
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _playerDetectRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _playerSafeRange);
    }
}
