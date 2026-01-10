using UnityEngine;
using UnityEngine.AI;

public abstract class BaseMonsterSpawner : MonoBehaviour
{
    [SerializeField] protected PoolManager _poolManager;

    protected GameObject SpawnMonster(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        Vector3 navMeshPosition = GetNavMeshPosition(position);
        GameObject monster = _poolManager.Get(prefab, navMeshPosition, rotation);
        SubscribeOnDie(monster);
        return monster;
    }

    private void SubscribeOnDie(GameObject monster)
    {
        var controllers = monster.GetComponentsInChildren<BadMonsterController>();
        foreach (var controller in controllers)
        {
            controller.OnDie += HandleMonsterDie;
        }
    }

    private void HandleMonsterDie(BadMonsterController controller)
    {
        controller.OnDie -= HandleMonsterDie;
        _poolManager.Return(controller.gameObject);
    }

    protected Vector3 GetNavMeshPosition(Vector3 position)
    {
        if (NavMesh.SamplePosition(position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return position;
    }
}
