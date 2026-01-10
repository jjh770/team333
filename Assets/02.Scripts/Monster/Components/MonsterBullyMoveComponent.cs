using UnityEngine;

public class MonsterBullyMoveComponent : MonsterMoveComponent
{
    [Header("Detection")]
    [SerializeField] private float _detectionRange = 30f;

    

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
        if (_target == null)
        {
            return;
        }

        float sqrDistance = (_target.position - transform.position).sqrMagnitude;

        if (sqrDistance <= _detectionRange * _detectionRange)
        {
            LookAtTarget();
        }
    }
}
