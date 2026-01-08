using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class MonsterSpawnTrigger : MonoBehaviour
{
    [Header("Spawn Group Index")]
    [SerializeField] private int _spawnGroupIndex;

    private BoxCollider _collider;
    private const string FloraTag = "Flora";

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(FloraTag)) return;

        SpawnMonsters();
    }

    private void SpawnMonsters()
    {
        MonsterPool.Instance.SpawnGroup(_spawnGroupIndex);
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
