using UnityEngine;

public class Flora : MonoBehaviour
{
    [SerializeField] private FloraStats _stats;
    [SerializeField] private FloraMovement _movement;
    [SerializeField] private SplineWaypointPath _pathProvider;
    [SerializeField] private FloraAnimationController _animationController;

    private void Awake()
    {
        _movement.Initialize(_stats, (IFloraPath)_pathProvider, _animationController);
    }
}
