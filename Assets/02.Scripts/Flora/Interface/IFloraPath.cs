using System;
using UnityEngine;

public interface IFloraPath
{
    Vector3 GetCurrentPoint();
    bool MoveNext();
    bool IsFinished { get; }
    bool ShouldWait { get; }
    
    event Action OnPathCompleted;
}