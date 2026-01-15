using UnityEngine;

public class PetMovementComponent : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private Vector3 _followOffset = new Vector3(-1.2f, 0f, -1.2f);
    [SerializeField] private float _rotationSpeed = 0.5f;

    private Transform _playerTransform;
    private bool _isInitialized;

    private float _movingSqrThreshold = 0.0001f;
    private float _lookDirectionSqrThreshold = 0.01f;

    public bool IsMoving { get; private set; }

    public void Initialize(Transform player)
    {
        _playerTransform = player;
        _isInitialized = true;
    }

    public void FollowPlayer(float dt)
    {
        Vector3 targetPosition = _playerTransform.position + _followOffset;

        Vector3 previousPosition = transform.position;
        transform.position = Vector3.Lerp(transform.position, targetPosition, dt);

        Vector3 delta = transform.position - previousPosition;
        IsMoving = delta.sqrMagnitude > _movingSqrThreshold;
    }

    public void LookPlayer()
    {
        if (_playerTransform == null) return;

        Vector3 direction = _playerTransform.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > _lookDirectionSqrThreshold)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed);
        }
    }
}
