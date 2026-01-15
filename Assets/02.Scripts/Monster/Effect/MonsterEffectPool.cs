using System.Collections;
using UnityEngine;

public class MonsterEffectPool : MonoBehaviour
{
    public static MonsterEffectPool Instance { get; private set; }

    [Header("Death Effects")]
    [SerializeField] protected GameObject _deathParticlePrefab;
    [SerializeField] protected float _deathParticleDuration = 3f;

    [Header("Hit Effects")]
    [SerializeField] protected GameObject _hitParticlePrefab;
    [SerializeField] protected float _hitParticleDuration = 3f;

    [Header("Settings")]
    [SerializeField] private int _preloadCount = 10;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        PreloadAllEffects();
    }

    private void PreloadAllEffects()
    {
        if (_deathParticlePrefab != null && PoolManager.Instance != null)
            PoolManager.Instance.Preload(_deathParticlePrefab, _preloadCount);
    }

    public GameObject PlaySmokeEffect(Vector3 position)
    {
        return PlayEffectWithDuration(_deathParticlePrefab, position, Quaternion.identity, _deathParticleDuration);
    }

    public GameObject PlayHitEffect(Vector3 position)
    {
        return PlayEffectWithDuration(_hitParticlePrefab, position, Quaternion.identity, _hitParticleDuration);
    }

    private GameObject PlayEffect(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null || PoolManager.Instance == null)
            return null;

        return PoolManager.Instance.Get(prefab, position, rotation);
    }

    private GameObject PlayEffectWithDuration(GameObject prefab, Vector3 position, Quaternion rotation, float duration)
    {
        GameObject effect = PlayEffect(prefab, position, rotation);

        if (effect != null && duration > 0)
        {
            StartCoroutine(ReturnAfterDelay(effect, duration));
        }

        return effect;
    }

    private IEnumerator ReturnAfterDelay(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnEffect(effect);
    }

    public void ReturnEffect(GameObject effect)
    {
        if (PoolManager.Instance != null && effect != null)
        {
            PoolManager.Instance.Return(effect);
        }
    }
}
