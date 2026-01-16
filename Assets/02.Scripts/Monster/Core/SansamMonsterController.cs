using UnityEngine;
using UnityEngine.AI;

public class SansamMonsterController : BadMonsterController
{
    private enum SansamBehavior
    {
        FollowFlora,
        AvoidPlayerToFlora
    }

    [Header("Sansam Settings")]
    [SerializeField] private float _playerDetectRange = 6f;
    [SerializeField] private float _playerSafeRange = 10f;
    [SerializeField] private float _fleeSpeedMultiplier = 1.5f;
    [SerializeField] private float _avoidDistance = 5f;
    [SerializeField] private float _detectionInterval = 0.2f;

    private Transform _floraTransform;
    private Transform _playerTransform;
    private NavMeshAgent _agent;

    private SansamBehavior _behavior = SansamBehavior.FollowFlora;
    private float _detectionTimer;
    private float _baseSpeed;

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

        Vector3 toPlayer = _playerTransform.position - transform.position;
        toPlayer.y = 0f;

        Vector3 toFlora = _floraTransform.position - transform.position;
        toFlora.y = 0f;

        // 플레이어 방향과 수직인 방향 계산. 좌우 중 플로라에 가까운 쪽
        Vector3 perpendicular = Vector3.Cross(toPlayer.normalized, Vector3.up);

        // 두 수직 방향 중 플로라 방향에 더 가까운 쪽 선택
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
            if (_agent != null && _agent.velocity.sqrMagnitude > 0.01f)
            {
                return MonsterState.Move;
            }
        }

        if (_move.IsMoving) return MonsterState.Move;
        return MonsterState.Idle;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _playerDetectRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _playerSafeRange);
    }
}
