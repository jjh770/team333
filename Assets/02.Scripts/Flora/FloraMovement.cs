using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class FloraMovement : MonoBehaviour
{
    private NavMeshAgent _agent;
    private FloraStats _stats;
    private IFloraPath _path;

    public void Initialize(FloraStats stats, IFloraPath path)
    {
        _stats = stats;
        _path = path;
        _agent = GetComponent<NavMeshAgent>();

        if (_stats != null)
        {
            _stats.SpeedChanged += OnSpeedChanged;
            _agent.speed = _stats.CurrentSpeed;
        }
        
        SetNextDestination();
    }

    private void OnDisable()
    {
        if (_stats != null)
            _stats.SpeedChanged -= OnSpeedChanged;
    }

    private void Update()
    {
        if (_path.IsFinished)
            return;

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            SetNextDestination();
        }
    }

    private void SetNextDestination()
    {
        if (_path.IsFinished)
            return;

        Vector3 target = _path.GetCurrentPoint();
        _agent.SetDestination(target);
        _path.MoveNext();
    }

    private void OnSpeedChanged(float current)
    {
        _agent.speed = current;
    }
}
