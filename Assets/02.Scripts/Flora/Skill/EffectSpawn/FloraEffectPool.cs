using System;
using System.Collections;
using UnityEngine;

public class FloraEffectPool : MonoBehaviour
{
    [Header("Lightning Effects")]
    [SerializeField] private GameObject _lightningStrikePrefab; 
    [SerializeField] private GameObject _lightningHitPrefab; 
    
    [Header("Slow Effects")]
    [SerializeField] private GameObject _slowEffectPrefab;
    
    [Header("Burn Effects")]
    [SerializeField] private GameObject _burnEffectPrefab;
    
    [Header("Settings")]
    [SerializeField] private int _preloadCount = 10;

    [Header("Dependencies")]
    [SerializeField] private MonoBehaviour _poolManagerComponent;

    private IPoolManager _poolManager;

    private void Awake()
    {
        if (_poolManagerComponent is not IPoolManager manager)
        {
            Debug.LogError("할당된 PoolManager가 IPoolManager를 구현하지 않았습니다.", this);
            return;
        }
        _poolManager = manager;
    }

    private void Start()
    {
        PreloadAllEffects();
    }

    private void PreloadAllEffects()
    {
        if (_lightningStrikePrefab != null)
            _poolManager.Preload(_lightningStrikePrefab, _preloadCount);
        
        if (_lightningHitPrefab != null)
            _poolManager.Preload(_lightningHitPrefab, _preloadCount);
        
        if (_slowEffectPrefab != null)
            _poolManager.Preload(_slowEffectPrefab, _preloadCount);
        
        if (_burnEffectPrefab != null)
            _poolManager.Preload(_burnEffectPrefab, _preloadCount);
    }

    public GameObject PlayLightningStrike(Vector3 position, float duration = 1f)
    {
        return PlayEffectWithDuration(_lightningStrikePrefab, position, Quaternion.identity, duration);
    }

    public GameObject PlayLightningHit(Vector3 position, float duration)
    {
        return PlayEffectWithDuration(_lightningHitPrefab, position, Quaternion.identity, duration);
    }

    public GameObject PlaySlowEffect(Vector3 position)
    {
        return PlayEffect(_slowEffectPrefab, position, Quaternion.identity);
    }

    public GameObject PlayBurnEffect(Vector3 position)
    {
        return PlayEffect(_burnEffectPrefab, position, Quaternion.identity);
    }

    private GameObject PlayEffect(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null || _poolManager == null)
            return null;

        return _poolManager.Get(prefab, position, rotation);
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

    public void ReturnEffect(GameObject effectObj)
    {
        if (_poolManager != null && effectObj != null)
        {
            _poolManager.Return(effectObj);
        }
    }

    private IEnumerator ReturnAfterDelay(GameObject effectObj, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnEffect(effectObj);
    }
}
