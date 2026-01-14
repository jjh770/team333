using UnityEngine;

[RequireComponent(typeof(FloraSpeedGaugeController))]
public class FloraDamageReceiver : MonoBehaviour, IDamageable
{
    private FloraSpeedGaugeController _speedGaugeController;
    private const float DamageToGaugeRatio = 100f;
    
    private void Awake()
    {
        _speedGaugeController = GetComponent<FloraSpeedGaugeController>();
    }

    public bool TryTakeDamage(Damage damage)
    {
        if (damage.Value <= 0)
            return false;

        _speedGaugeController.DrainGauge(damage.Value / DamageToGaugeRatio);
        return true;
    }
}
