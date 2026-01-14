using UnityEngine;

public class MonsterPlacedMoveComponent : MonsterMoveComponent
{
    [Header("Detection")]
    [SerializeField] private float _detectionRange = 10f;

    public override bool CanKnockback => false;

    protected void Awake()
    {
        _isMoving = false;
    }

    public override void UpdateMove()
    {
        TryLookAtTarget();
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
}
