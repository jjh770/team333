using UnityEngine;

public class PlayerMouseHelper : MonoBehaviour
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

    // 마우스 월드 좌표를 가져오는 헬퍼 메서드
    public Vector3 GetMouseWorldPosition()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        // 플레이어의 발밑 높이(y=0 또는 transform.position.y)를 기준으로 평면 생성
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));

        if (groundPlane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }
        return transform.position; // 실패 시 현재 위치 반환
    }
}
