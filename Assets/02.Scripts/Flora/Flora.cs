using UnityEngine;

public class Flora : MonoBehaviour
{
    [SerializeField] private FloraStats _stats;
    [SerializeField] private FloraMovement _movement;
    [SerializeField] private WaypointPath _pathProvider;

    private void Awake()
    {
        _movement.Initialize(_stats, (IFloraPath)_pathProvider);
    }
}
