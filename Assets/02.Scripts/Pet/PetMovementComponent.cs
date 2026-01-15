using UnityEngine;

public class PetMovementComponent : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private Vector3 _followOffset = new Vector3(-1.2f, 0f, -1.2f);
    [SerializeField] private float _rotationSpeed = 0.5f;

    private Transform _playerTransform;
    private bool _isInitialized;

    public void Initialize(Transform player)
    {
        _playerTransform = player;
        _isInitialized = true;
    }



    public void FollowPlayer(float dt)
    {
        Vector3 targetPosition = _playerTransform.position + _followOffset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, dt);
    }

    public void LookPlayer()
    {
        if (_playerTransform == null) return;

        Vector3 direction = _playerTransform.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed);
        }
    }
}
