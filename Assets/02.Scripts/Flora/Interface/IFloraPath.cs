using System;
using UnityEngine;

public interface IFloraPath
{
    Vector3 GetCurrentPoint();
    bool MoveNext();
    bool IsFinished { get; }
    bool ShouldWait { get; }
    float Progress { get; }

    event Action OnPathCompleted;
    event Action<float> OnProgressChanged;
}