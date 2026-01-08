using UnityEngine;

[RequireComponent(typeof(FloraSpeedGaugeController))]
public class FloraDamageReceiver : MonoBehaviour, IDamageable
{
    private FloraSpeedGaugeController _speedGaugeController;

    private void Awake()
    {
        _speedGaugeController = GetComponent<FloraSpeedGaugeController>();
    }

    public bool TryTakeDamage(Damage damage)
    {
        if (damage.Value <= 0)
            return false;

        _speedGaugeController.DrainGauge(damage.Value);
        return true;
    }
}
