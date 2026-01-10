using UnityEngine;

public class FloraFollowCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;

    [Header("Position")]
    [SerializeField] private Vector3 _localOffset = new Vector3(5f, 10f, -5f);
    [SerializeField] private float _positionSmoothSpeed = 5f;

    [Header("Look Ahead")]
    [SerializeField] private float _lookAheadDistance = 1f;
    [SerializeField] private float _lookAheadSmoothSpeed = 3f;

    [Header("Rotation")]
    [SerializeField] private float _rotationSmoothSpeed = 5f;

    private Vector3 _currentLookAhead;
    private Vector3 _lastTargetPosition;

    private void Start()
    {
        if (enabled)
        {
            ResetLastTargetPosition();
        }
    }

    private void LateUpdate()
    {
        if (!enabled || _target == null)
            return;

        UpdateCamera();
    }
    
    private void OnEnable()
    {
        ResetLastTargetPosition();
    }
    
    private void UpdateCamera()
    {
        Vector3 worldOffset = CalculateWorldOffset();
        Vector3 velocity    = CalculateTargetVelocity();

        UpdateLookAhead(velocity);
        UpdatePosition(worldOffset);
        UpdateRotation();
    }
    
    
    private Vector3 CalculateWorldOffset()
    {
        return _target.TransformDirection(_localOffset);
    }

    private Vector3 CalculateTargetVelocity()
    {
        Vector3 velocity = Vector3.zero;

        if (Time.deltaTime > Mathf.Epsilon)
        {
            velocity = (_target.position - _lastTargetPosition) / Time.deltaTime;
        }

        _lastTargetPosition = _target.position;
        return velocity;
    }
    
    private void UpdateLookAhead(Vector3 velocity)
    {
        Vector3 targetLookAhead = velocity.normalized * _lookAheadDistance;

        _currentLookAhead = Vector3.Lerp(
            _currentLookAhead,
            targetLookAhead,
            _lookAheadSmoothSpeed * Time.deltaTime
        );
    }
    
    private void UpdatePosition(Vector3 worldOffset)
    {
        Vector3 desiredPosition =
            _target.position + worldOffset + _currentLookAhead;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            _positionSmoothSpeed * Time.deltaTime
        );
    }
    
    private void UpdateRotation()
    {
        Vector3 lookAtPoint = _target.position + _currentLookAhead;

        Quaternion targetRotation =
            Quaternion.LookRotation(lookAtPoint - transform.position);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            _rotationSmoothSpeed * Time.deltaTime
        );
    }
    
    public void SetTarget(Transform target)
    {
        _target = target;
        ResetLastTargetPosition();
    }
    
    private void ResetLastTargetPosition()
    {
        if (_target != null)
        {
            _lastTargetPosition = _target.position;
        }
    }
    
    public Vector3 GetLocalOffset()
    {
        return _localOffset;
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying && _target != null)
        {
            Vector3 worldOffset = _target.TransformDirection(_localOffset);
            transform.position = _target.position + worldOffset;
            transform.LookAt(_target);
        }
    }
#endif
}