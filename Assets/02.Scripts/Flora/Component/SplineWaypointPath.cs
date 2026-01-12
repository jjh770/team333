using System;
using System.Collections.Generic;
using UnityEngine;

public class SplineWaypointPath : MonoBehaviour, IFloraPath
{
    [SerializeField] private Transform[] _waypoints;
    [SerializeField, Range(1, 20)] private int _resolution = 10;
    [SerializeField] private int[] _waitPointIndexes;

    private List<Vector3> _splinePoints;
    private HashSet<int> _waitPointIndex;
    private int _currentIndex;
    private bool _isCompleted;

    public bool IsFinished => _currentIndex >= _splinePoints.Count;
    public bool ShouldWait => _waitPointIndex != null && _waitPointIndex.Contains(_currentIndex);
    public float Progress => _isCompleted ? 1f : (_splinePoints.Count <= 1 ? 0f : Mathf.Clamp01((float)_currentIndex / (_splinePoints.Count - 1)));

    public event Action OnPathCompleted;
    public event Action<float> OnProgressChanged;
    
    private void Awake()
    {
        GenerateSplinePoints();
    }

    public Vector3 GetCurrentPoint()
    {
        return _splinePoints[_currentIndex];
    }

    public bool MoveNext()
    {
        _currentIndex++;
        bool hasNext = _currentIndex < _splinePoints.Count;

        OnProgressChanged?.Invoke(Progress);

        if (!hasNext && !_isCompleted)
        {
            _isCompleted = true;
            OnPathCompleted?.Invoke();
        }

        return hasNext;
    }

    private void GenerateSplinePoints()
    {
        _splinePoints = new List<Vector3>();
        _waitPointIndex = new HashSet<int>();

        if (_waypoints == null || _waypoints.Length < 2)
        {
            return;
        }

        for (int i = 0; i < _waypoints.Length - 1; i++)
        {
            if (Array.IndexOf(_waitPointIndexes, i) >= 0)
            {
                _waitPointIndex.Add(_splinePoints.Count);
            }

            Vector3 p0 = GetWaypointPosition(i - 1);
            Vector3 p1 = GetWaypointPosition(i);
            Vector3 p2 = GetWaypointPosition(i + 1);
            Vector3 p3 = GetWaypointPosition(i + 2);

            for (int j = 0; j < _resolution; j++)
            {
                float t = j / (float)_resolution;
                Vector3 point = CatmullRom(p0, p1, p2, p3, t);
                _splinePoints.Add(point);
            }
        }

        if (Array.IndexOf(_waitPointIndexes, _waypoints.Length - 1) >= 0)
        {
            _waitPointIndex.Add(_splinePoints.Count);
        }

        _splinePoints.Add(_waypoints[^1].position);
    }

    private Vector3 GetWaypointPosition(int index)
    {
        if (index < 0)
            return _waypoints[0].position;
        if (index >= _waypoints.Length)
            return _waypoints[^1].position;
        return _waypoints[index].position;
    }

    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            2f * p1 +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_waypoints == null || _waypoints.Length < 2)
            return;

        List<Vector3> previewPoints = GeneratePreviewPoints();

        Gizmos.color = Color.cyan;
        for (int i = 0; i < previewPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(previewPoints[i], previewPoints[i + 1]);
        }

        Gizmos.color = Color.yellow;
        foreach (var waypoint in _waypoints)
        {
            if (waypoint != null)
                Gizmos.DrawSphere(waypoint.position, 0.3f);
        }
    }

    private List<Vector3> GeneratePreviewPoints()
    {
        var points = new List<Vector3>();

        for (int i = 0; i < _waypoints.Length - 1; i++)
        {
            if (_waypoints[i] == null || _waypoints[i + 1] == null)
                continue;

            Vector3 p0 = GetWaypointPositionEditor(i - 1);
            Vector3 p1 = GetWaypointPositionEditor(i);
            Vector3 p2 = GetWaypointPositionEditor(i + 1);
            Vector3 p3 = GetWaypointPositionEditor(i + 2);

            for (int j = 0; j <= _resolution; j++)
            {
                float t = j / (float)_resolution;
                points.Add(CatmullRom(p0, p1, p2, p3, t));
            }
        }

        return points;
    }

    private Vector3 GetWaypointPositionEditor(int index)
    {
        if (index < 0)
        {
            return (_waypoints.Length > 0 && _waypoints[0] != null) ? _waypoints[0].position : Vector3.zero;
        }

        if (index >= _waypoints.Length)
        {
            return (_waypoints.Length > 0 && _waypoints[^1] != null) ? _waypoints[^1].position : Vector3.zero;
        }

        return _waypoints[index] != null ? _waypoints[index].position : Vector3.zero;
    }
#endif
}
