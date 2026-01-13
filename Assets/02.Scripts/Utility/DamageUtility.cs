using UnityEngine;

public static class DamageUtility
{
    public static void ApplyDamage(GameObject target, float damage, GameObject source, bool isPlayerDamage = true)
    {
        Damage takeDamage = new Damage(damage, source, isPlayerDamage);
        if (target.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TryTakeDamage(takeDamage);
        }
    }
}
