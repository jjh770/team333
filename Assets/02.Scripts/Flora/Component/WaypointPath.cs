using UnityEngine;

public class WaypointPath : MonoBehaviour, IFloraPath
{
    [SerializeField] private Transform[] _points;

    private int _currentIndex;

    public bool IsFinished => _currentIndex >= _points.Length;

    public Vector3 GetCurrentPoint()
    {
        return _points[_currentIndex].position;
    }

    public bool MoveNext()
    {
        if (_currentIndex + 1 >= _points.Length)
            return false;

        _currentIndex++;
        return true;
    }
    
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_points == null || _points.Length < 2)
            return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < _points.Length - 1; i++)
        {
            if (_points[i] == null || _points[i + 1] == null)
                continue;

            Gizmos.DrawLine(
                _points[i].position,
                _points[i + 1].position
            );

            Gizmos.DrawSphere(_points[i].position, 0.3f);
        }

        Gizmos.DrawSphere(_points[^1].position, 0.3f);
    }
#endif
}
