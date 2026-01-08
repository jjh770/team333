using UnityEngine;

public class MonsterPlacedMoveComponent : MonsterMoveComponent
{
    [Header("Detection")]
    [SerializeField] private float _detectionRange = 10f;

    protected override void Awake()
    {
        base.Awake();
        _agent.isStopped = true;
        _agent.updatePosition = false;
    }

    public override void UpdateMove()
    {
        TryLookAtTarget();
        _isMoving = false;
    }

    private void TryLookAtTarget()
    {
        if (_target == null) return;

        float sqrDistance = (_target.position - transform.position).sqrMagnitude;

        if (sqrDistance <= _detectionRange * _detectionRange)
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
