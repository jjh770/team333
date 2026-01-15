using UnityEngine;

public class PlayerEffectPool : MonoBehaviour
{
    public static PlayerEffectPool Instance { get; private set; }

    [Header("Skill Projectile")]
    [SerializeField] private GameObject _skillProjectilePrefab;

    [Header("Settings")]
    [SerializeField] private int _preloadCount = 5;

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
        if (_skillProjectilePrefab != null && PoolManager.Instance != null)
        {
            PoolManager.Instance.Preload(_skillProjectilePrefab, _preloadCount);
        }
    }

    public SkillProjectile GetSkillProjectile(Vector3 position, Quaternion rotation)
    {
        if (_skillProjectilePrefab == null || PoolManager.Instance == null)
            return null;

        GameObject obj = PoolManager.Instance.Get(_skillProjectilePrefab, position, rotation);
        return obj.GetComponent<SkillProjectile>();
    }

    public void ReturnSkillProjectile(GameObject projectile)
    {
        if (PoolManager.Instance != null && projectile != null)
        {
            PoolManager.Instance.Return(projectile);
        }
    }
}
