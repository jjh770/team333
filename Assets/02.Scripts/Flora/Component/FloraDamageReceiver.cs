using UnityEngine;

[RequireComponent(typeof(FloraSpeedUpController))]
public class FloraDamageReceiver : MonoBehaviour, IDamageable
{
    private FloraSpeedUpController _speedUpController;

    private void Awake()
    {
        _speedUpController = GetComponent<FloraSpeedUpController>();
    }

    public bool TryTakeDamage(Damage damage)
    {
        if (damage.Value <= 0)
            return false;

        _speedUpController.DrainGauge(damage.Value);
        return true;
    }
}
