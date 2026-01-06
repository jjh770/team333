using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class TraceMoveComponent : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 4f;
    [SerializeField] private float _updateInterval = 0.2f;

    private NavMeshAgent agent;
    private Transform player;
    private float updateTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = _moveSpeed;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        updateTimer -= Time.deltaTime;
        if (updateTimer <= 0f)
        {
            agent.SetDestination(player.position);
            updateTimer = _updateInterval;
        }
    }
}
