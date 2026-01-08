using UnityEngine;

public class MonsterPlacedMoveComponent : MonsterMoveComponent
{
    [Header("Detection")]
    [SerializeField] private float _detectionRange = 10f;

    protected override void Start()
    {
        base.Start();
        _agent.isStopped = true;
        _agent.updatePosition = false;
    }

    private void Update()
    {
        TryLookAtPlayer();
    }

    private void TryLookAtPlayer()
    {
        if (_player == null) return;

        float sqrDistanceToPlayer = (_player.position - transform.position).sqrMagnitude;

        if (sqrDistanceToPlayer <= _detectionRange * _detectionRange)
        {
            LookAtTarget();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }
}
