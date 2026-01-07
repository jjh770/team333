using UnityEngine;

public class FloraGaugeInput : MonoBehaviour
{
    [SerializeField] private FloraSpeedUpController _speedUpController;
    [SerializeField] private float _gaugeAmount = 0.1f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _speedUpController.AddGauge(_gaugeAmount);
        }
    }
}