using UnityEngine;

public class MonsterPlacedMoveComponent : MonsterMoveComponent
{
    [Header("Detection")]
    [SerializeField] private float _detectionRange = 10f;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 5f;

    private const float MinLookDirectionSqrMagnitude = 1e-6f;

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
            LookAtPlayer();
        }
    }

    private void LookAtPlayer()
    {
        Vector3 direction = _player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > MinLookDirectionSqrMagnitude)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }
}
