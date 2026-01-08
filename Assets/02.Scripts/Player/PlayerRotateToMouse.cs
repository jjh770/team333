using UnityEngine;

public class PlayerRotateToMouse : MonoBehaviour
{
    private Camera _mainCamera;
    [Header("Mouse Rotation")]
    [SerializeField] private float _rotationSpeed = 300f;

    private void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }
    }

    public void RotateTowardsMouse()
    {
        Plane groundPlane = new Plane(Vector3.up, transform.position);
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 targetPosition = ray.GetPoint(distance);
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }
        }
    }
}
