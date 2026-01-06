using UnityEngine;

public class Flora : MonoBehaviour
{
    private FloraStats _stats;
    private FloraMovement _movement;
    private WaypointPath _pathProvider;

    private void Start()
    {
        _stats = GetComponent<FloraStats>();
        _movement = GetComponent<FloraMovement>();
        _pathProvider = GetComponent<WaypointPath>();
        
        _movement.Initialize(_stats, (IFloraPath)_pathProvider);
    }
}
