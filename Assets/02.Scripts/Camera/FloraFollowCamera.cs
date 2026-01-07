using UnityEngine;

public class FloraFollowCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset = new Vector3(-3.5f, 10f, -4.5f);
    [SerializeField] private float _smoothSpeed = 5f;

    [Header("Camera Angle")]
    [SerializeField, Range(45f, 90f)] private float _pitch = 60f;

    private void LateUpdate()
    {
        if (_target == null)
            return;

        Vector3 desiredPosition = _target.position + _offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        transform.rotation = Quaternion.Euler(_pitch, 45f, 0f);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying && _target != null)
        {
            transform.position = _target.position + _offset;
            transform.rotation = Quaternion.Euler(_pitch, 45f, 0f);
        }
    }
#endif
}