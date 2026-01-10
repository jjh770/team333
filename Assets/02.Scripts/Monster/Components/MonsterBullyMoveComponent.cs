using UnityEngine;

public class MonsterBullyMoveComponent : MonsterMoveComponent
{
    [Header("Detection")]
    [SerializeField] private float _detectionRange = 10f;

    [SerializeField] private GameObject _bullyTarget;

    protected void Awake()
    {
        _isMoving = false;
    }

    private void Start()
    {
        _target = _bullyTarget.transform;
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
