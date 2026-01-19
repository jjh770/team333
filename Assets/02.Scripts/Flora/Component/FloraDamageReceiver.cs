using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(FloraSpeedGaugeController))]
public class FloraDamageReceiver : MonoBehaviour, IDamageable
{
    [Header("Hit Effect")]
    [SerializeField] private SkinnedMeshRenderer _renderer;
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private float _flashDuration = 0.3f;

    private FloraSpeedGaugeController _speedGaugeController;
    private Material _material;
    private Tweener _flashTween;
    private const float DamageToGaugeRatio = 100f;

    private static readonly int s_emissionColor = Shader.PropertyToID("_Emissive_Color");

    private void Awake()
    {
        _speedGaugeController = GetComponent<FloraSpeedGaugeController>();

        if (_renderer != null)
        {
            _material = _renderer.material;
        }
    }

    private void OnDisable()
    {
        _flashTween?.Kill();

        if (_material != null)
        {
            _material.SetColor(s_emissionColor, Color.black);
        }
    }

    public bool TryTakeDamage(Damage damage)
    {
        if (damage.Value <= 0)
            return false;

        _speedGaugeController.DrainGauge(damage.Value / DamageToGaugeRatio);
        FlashWhite();
        return true;
    }

    private void FlashWhite()
    {
        if (_material == null) return;

        _flashTween?.Kill();
        _material.SetColor(s_emissionColor, _flashColor);
        _flashTween = _material.DOColor(Color.black, s_emissionColor, _flashDuration);
    }
}
