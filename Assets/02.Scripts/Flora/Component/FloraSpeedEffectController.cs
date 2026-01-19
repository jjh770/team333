using UnityEngine;

public class FloraSpeedEffectController : MonoBehaviour
{
     [Header("Reference")]
    [SerializeField] private FloraSpeedGaugeController _gaugeController;
    [SerializeField] private FloraSound _floraSound;

    [Header("Particles (Children)")]
    [SerializeField] private ParticleSystem[] _particles;

    [Header("Rule")]
    [Range(0f, 1f)]
    [SerializeField] private float _activeThreshold = 0.5f;

    private bool _isActive;

    private void Awake()
    {
        if (_particles == null || _particles.Length == 0)
        {
            _particles = GetComponentsInChildren<ParticleSystem>(true);
        }
        if (_gaugeController == null)
        {
            enabled = false;
        }
    }

    private void Start()
    {
        StopParticlesImmediate();
    }

    private void OnEnable()
    {
        _gaugeController.GaugeChanged += OnGaugeChanged;
    }

    private void OnDisable()
    {
        _gaugeController.GaugeChanged -= OnGaugeChanged;
    }

    private void OnGaugeChanged(float current, float max)
    {
        float percent = current / max;
        bool shouldActive = percent >= _activeThreshold;

        if (shouldActive == _isActive)
            return;

        _isActive = shouldActive;

        if (_isActive)
        {
            PlayParticles();
        }
        else
        {
            StopParticlesSmooth();
        }
    }

    private void PlayParticles()
    {
        _floraSound?.PlaySpeedUp();

        foreach (var ps in _particles)
        {
            if (ps == null) continue;

            if (!ps.isPlaying)
            {
                ps.Play();
            }
        }
    }

    private void StopParticlesSmooth()
    {
        foreach (var ps in _particles)
        {
            if (ps == null) continue;

            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    private void StopParticlesImmediate()
    {
        foreach (var ps in _particles)
        {
            if (ps == null) continue;

            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
