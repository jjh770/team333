using UnityEngine;

public class FloraSkillHolderTerrainAligner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FloraSkillController _skillController;

    [Header("Terrain Alignment Settings")]
    [SerializeField] private LayerMask _groundLayer = ~0;
    [SerializeField] private float _raycastHeight = 10f;
    [SerializeField] private float _raycastDistance = 20f;
    [SerializeField] private float _heightOffset = 0.1f;
    [SerializeField] private float _smoothSpeed = 5f;
    [SerializeField] private bool _smoothTransition = true;

    [Header("Debug / Preview")]
    [SerializeField] private float _previewRadius = 4f;
    
    private const int SamplePointCount = 5;
    private Vector3 _targetPosition;
    private Quaternion _targetRotation;

    private float CurrentRadius
    {
        get
        {
            if (_skillController != null && _skillController.HasSkill)
                return _skillController.CurrentSkill.Radius;
            return _previewRadius;
        }
    }

    private Vector3 CenterPosition
    {
        get
        {
            if (_skillController != null)
                return _skillController.transform.position;
            return transform.parent != null ? transform.parent.position : transform.position;
        }
    }

    private void Update()
    {
        if (_skillController == null)
        {
            return;
        }

        if (!_skillController.HasSkill)
        {
            return;
        }

        AlignToTerrain();
    }

    private void AlignToTerrain()
    {
        float radius = CurrentRadius;
        Vector3 center = CenterPosition;

        // 상하좌우 4개의 점 + 중앙 (위에서 봤을 때 forward, back, left, right, center)
        Vector3[] sampleOffsets = new Vector3[]
        {
            Vector3.forward * radius,  // 앞 (상)
            Vector3.back * radius,     // 뒤 (하)
            Vector3.left * radius,     // 좌
            Vector3.right * radius,    // 우
            Vector3.zero               // 중앙
        };

        Vector3[] worldPoints = new Vector3[SamplePointCount];
        float[] heights = new float[SamplePointCount];
        int validCount = 0;

        for (int i = 0; i < SamplePointCount; i++)
        {
            Vector3 samplePos = center + sampleOffsets[i];
            Vector3 rayOrigin = samplePos + Vector3.up * _raycastHeight;

            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, _raycastDistance, _groundLayer))
            {
                worldPoints[i] = hit.point;
                heights[i] = hit.point.y;
                validCount++;
            }
            else
            {
                worldPoints[i] = samplePos;
                heights[i] = center.y;
            }
        }

        if (validCount < 3)
        {
            return;
        }

        // 최대 높이 계산 (땅에 박히지 않도록)
        float maxHeight = heights[0];
        for (int i = 1; i < 5; i++)
        {
            if (heights[i] > maxHeight)
                maxHeight = heights[i];
        }

        // 법선 벡터 계산 (전후좌우 4개의 점으로)
        Vector3 forwardDir = (worldPoints[0] - worldPoints[1]).normalized;
        Vector3 rightDir = (worldPoints[3] - worldPoints[2]).normalized;

        // 두 방향의 외적으로 법선 계산
        Vector3 normal = Vector3.Cross(rightDir, forwardDir).normalized;

        // 법선이 아래를 향하면 반전
        if (normal.y < 0)
        {
            normal = -normal;
        }

        // 목표 위치 계산 (최대 높이 기준 + 오프셋)
        _targetPosition = transform.localPosition;
        _targetPosition.y = (maxHeight + _heightOffset) - CenterPosition.y;

        // 목표 회전 계산 (법선 방향으로)
        _targetRotation = Quaternion.FromToRotation(Vector3.up, normal);

        // 적용
        if (_smoothTransition)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _targetPosition, Time.deltaTime * _smoothSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * _smoothSpeed);
        }
        else
        {
            transform.localPosition = _targetPosition;
            transform.rotation = _targetRotation;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        float radius = CurrentRadius;
        Vector3 center = CenterPosition;

        // 샘플링 포인트 표시
        Vector3[] offsets = new Vector3[]
        {
            Vector3.forward * radius,
            Vector3.back * radius,
            Vector3.left * radius,
            Vector3.right * radius,
            Vector3.zero
        };

        Gizmos.color = Color.cyan;
        foreach (var offset in offsets)
        {
            Vector3 point = center + offset;
            Gizmos.DrawWireSphere(point, 0.3f);
            Gizmos.DrawLine(point + Vector3.up * _raycastHeight, point - Vector3.up * (_raycastDistance - _raycastHeight));
        }

        // 현재 위치의 법선 방향 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.up * 2f);

        // 범위 원 표시
        Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
        DrawWireCircle(center, radius, 32);
    }

    private void DrawWireCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
#endif
}
