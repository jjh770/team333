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
        Debug.Log("UpdateMove");
    }

    private void TryLookAtTarget()
    {
        if (_target == null)
        {
            Debug.Log("_target == null");
            return;
        }

        float sqrDistance = (_target.position - transform.position).sqrMagnitude;

        if (sqrDistance <= _detectionRange * _detectionRange)
        {
            Debug.Log("TryLookAtTarget");
            LookAtTarget();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }
}
