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
        if (_target != null)
            _lastTargetPosition = _target.position;
    }

    private void LateUpdate()
    {
        if (_target == null)
            return;

        Vector3 worldOffset = _target.TransformDirection(_localOffset);

        Vector3 velocity = (_target.position - _lastTargetPosition) / Time.deltaTime;
        _lastTargetPosition = _target.position;

        Vector3 targetLookAhead = velocity.normalized * _lookAheadDistance;
        _currentLookAhead = Vector3.Lerp(_currentLookAhead, targetLookAhead, _lookAheadSmoothSpeed * Time.deltaTime);

        Vector3 desiredPosition = _target.position + worldOffset + _currentLookAhead;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, _positionSmoothSpeed * Time.deltaTime);

        Vector3 lookAtPoint = _target.position + _currentLookAhead;
        Quaternion targetRotation = Quaternion.LookRotation(lookAtPoint - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSmoothSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        if (_target != null)
            _lastTargetPosition = _target.position;
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