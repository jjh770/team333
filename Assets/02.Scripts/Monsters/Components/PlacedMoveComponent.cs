using UnityEngine;

public class PlacedMoveComponent : MoveComponent
{
    [Header("Detection")]
    [SerializeField] private float _detectionRange = 10f;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 5f;

    private const float MinLookDirectionSqrMagnitude = 1e-6f;

    protected override void Start()
    {
        base.Start();
        agent.isStopped = true;
    }

    void Update()
    {
        TryLookAtPlayer();
    }

    private void TryLookAtPlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= _detectionRange)
        {
            LookAtPlayer();
        }
    }

    private void LookAtPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > MinLookDirectionSqrMagnitude)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }
}
