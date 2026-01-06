using UnityEngine;

public interface IFloraPath
{
    Vector3 GetCurrentPoint();
    bool MoveNext();
    bool IsFinished { get; }
}