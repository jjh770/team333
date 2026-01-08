using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class MonsterSpawnTrigger : MonoBehaviour
{
    [Header("Spawn Group Index")]
    [SerializeField] private int _spawnGroupIndex;

    [Header("Dependencies")]
    [SerializeField] private MonoBehaviour _monsterSpawnerComponent;

    private IMonsterSpawner _monsterSpawner;
    private BoxCollider _collider;
    private const string FloraTag = "Flora";

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();

        if (_monsterSpawnerComponent is not IMonsterSpawner spawner)
        {
            Debug.LogError("할당된 컴포넌트가 IMonsterSpawner를 구현하지 않았습니다.", this);
            return;
        }
        _monsterSpawner = spawner;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(FloraTag)) return;

        SpawnMonsters();
    }

    private void SpawnMonsters()
    {
        if (_monsterSpawner == null) return;

        _monsterSpawner.SpawnGroup(_spawnGroupIndex);
        enabled = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        BoxCollider box = _collider != null ? _collider : GetComponent<BoxCollider>();
        if (box == null) return;

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(box.center, box.size);

        Gizmos.color = new Color(1f, 0.5f, 0f, 1f);
        Gizmos.DrawWireCube(box.center, box.size);
    }
#endif
}