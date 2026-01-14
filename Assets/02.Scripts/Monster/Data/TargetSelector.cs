using UnityEngine;

public enum TargetType
{
    Player,
    Flora,
    Both
}

public class TargetSelector : MonoBehaviour
{
    [Header("Settings")]
    public TargetType preferredTarget = TargetType.Player;

    private Transform _currentTargetTransform;
}
