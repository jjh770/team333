using UnityEngine;

/// <summary>
/// 카메라 경계 관련 유틸리티 헬퍼 클래스
/// 플레이어나 오브젝트의 위치를 카메라 뷰포트 범위 내로 제한하는 기능 제공
/// </summary>
public static class CameraBoundsHelper
{
    /// <summary>
    /// 오브젝트 위치를 카메라 뷰포트 범위 내로 클램핑
    /// </summary>
    /// <param name="transform">대상 Transform</param>
    /// <param name="controller">CharacterController (위치 수정 시 비활성화 필요)</param>
    /// <param name="camera">기준 카메라</param>
    /// <param name="viewportMargin">뷰포트 경계 마진 (0.0 ~ 1.0)</param>
    public static void ClampPositionToCameraBounds(Transform transform, CharacterController controller, Camera camera, float viewportMargin)
    {
        Vector3 viewportPos = camera.WorldToViewportPoint(transform.position);

        // 경계를 벗어난 경우만 처리
        bool isOutOfBounds = viewportPos.x < viewportMargin || viewportPos.x > 1f - viewportMargin ||
                             viewportPos.y < viewportMargin || viewportPos.y > 1f - viewportMargin ||
                             viewportPos.z <= 0;

        if (isOutOfBounds)
        {
            // 뷰포트 좌표를 마진 범위 내로 클램핑
            viewportPos.x = Mathf.Clamp(viewportPos.x, viewportMargin, 1f - viewportMargin);
            viewportPos.y = Mathf.Clamp(viewportPos.y, viewportMargin, 1f - viewportMargin);

            // 클램핑된 뷰포트 좌표를 월드 좌표로 변환
            Vector3 clampedWorldPos = camera.ViewportToWorldPoint(viewportPos);

            // CharacterController를 일시적으로 비활성화하고 위치 직접 수정
            controller.enabled = false;
            transform.position = new Vector3(clampedWorldPos.x, transform.position.y, clampedWorldPos.z);
            controller.enabled = true;
        }
    }

    /// <summary>
    /// 오브젝트 위치를 카메라 뷰포트 범위 내로 클램핑 (일반 GameObject용)
    /// </summary>
    /// <param name="transform">대상 Transform</param>
    /// <param name="camera">기준 카메라</param>
    /// <param name="viewportMargin">뷰포트 경계 마진 (0.0 ~ 1.0)</param>
    public static void ClampPositionToCameraBounds(Transform transform, Camera camera, float viewportMargin)
    {
        Vector3 viewportPos = camera.WorldToViewportPoint(transform.position);

        bool isOutOfBounds = viewportPos.x < viewportMargin || viewportPos.x > 1f - viewportMargin ||
                             viewportPos.y < viewportMargin || viewportPos.y > 1f - viewportMargin ||
                             viewportPos.z <= 0;

        if (isOutOfBounds)
        {
            viewportPos.x = Mathf.Clamp(viewportPos.x, viewportMargin, 1f - viewportMargin);
            viewportPos.y = Mathf.Clamp(viewportPos.y, viewportMargin, 1f - viewportMargin);

            Vector3 clampedWorldPos = camera.ViewportToWorldPoint(viewportPos);
            transform.position = new Vector3(clampedWorldPos.x, transform.position.y, clampedWorldPos.z);
        }
    }

    /// <summary>
    /// 이동 벡터를 카메라 경계 내로 제한
    /// </summary>
    /// <param name="currentPosition">현재 위치</param>
    /// <param name="moveVector">이동 벡터</param>
    /// <param name="camera">기준 카메라</param>
    /// <param name="viewportMargin">뷰포트 경계 마진 (0.0 ~ 1.0)</param>
    /// <returns>제한된 이동 벡터</returns>
    public static Vector3 ClampMovementToCameraBounds(Vector3 currentPosition, Vector3 moveVector, Camera camera, float viewportMargin)
    {
        Vector3 targetPosition = currentPosition + moveVector;
        Vector3 viewportPos = camera.WorldToViewportPoint(targetPosition);

        // 경계를 벗어나는지 확인
        bool wouldBeOutOfBounds = viewportPos.x < viewportMargin || viewportPos.x > 1f - viewportMargin ||
                                   viewportPos.y < viewportMargin || viewportPos.y > 1f - viewportMargin ||
                                   viewportPos.z <= 0;

        if (wouldBeOutOfBounds)
        {
            // 뷰포트 좌표를 마진 범위 내로 클램핑
            viewportPos.x = Mathf.Clamp(viewportPos.x, viewportMargin, 1f - viewportMargin);
            viewportPos.y = Mathf.Clamp(viewportPos.y, viewportMargin, 1f - viewportMargin);

            // 클램핑된 뷰포트 좌표를 월드 좌표로 변환
            Vector3 clampedWorldPos = camera.ViewportToWorldPoint(viewportPos);

            // 현재 위치에서 클램핑된 위치까지의 벡터 반환
            return new Vector3(clampedWorldPos.x, currentPosition.y, clampedWorldPos.z) - currentPosition;
        }

        return moveVector;
    }

    /// <summary>
    /// 현재 위치가 카메라 경계 내에 있는지 확인
    /// </summary>
    /// <param name="position">확인할 위치</param>
    /// <param name="camera">기준 카메라</param>
    /// <param name="viewportMargin">뷰포트 경계 마진 (0.0 ~ 1.0)</param>
    /// <returns>경계 내에 있으면 true, 벗어나면 false</returns>
    public static bool IsWithinCameraBounds(Vector3 position, Camera camera, float viewportMargin)
    {
        Vector3 viewportPos = camera.WorldToViewportPoint(position);

        return viewportPos.x >= viewportMargin && viewportPos.x <= 1f - viewportMargin &&
               viewportPos.y >= viewportMargin && viewportPos.y <= 1f - viewportMargin &&
               viewportPos.z > 0;
    }
}
