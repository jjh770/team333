using UnityEngine;

public class PlayerMouseHelper : MonoBehaviour
{
    private Camera _mainCamera;
    [Header("Mouse Rotation")]
    [SerializeField] private float _rotationSpeed = 300f;
    [SerializeField] private LayerMask _groundLayer;

    private void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }
    }

    public void RotateTowardsMouse()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPosition;

        // Ground Layer로 Raycast, 실패하면 Plane 사용
        if (_groundLayer != 0 && Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayer))
        {
            targetPosition = hit.point;
        }
        else
        {
            Plane groundPlane = new Plane(Vector3.up, transform.position);
            if (groundPlane.Raycast(ray, out float distance))
            {
                targetPosition = ray.GetPoint(distance);
            }
            else
            {
                return;
            }
        }

        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }

    public Vector3 GetMouseWorldPosition()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        // Ground Layer로 Raycast, 실패하면 Plane 사용
        if (_groundLayer != 0 && Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayer))
        {
            return hit.point;
        }

        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));
        if (groundPlane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }

        return transform.position;
    }
}
