using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class PooledParticle : MonoBehaviour, IPoolable
{
    private ParticleSystem _particle;

    private void Awake()
    {
        _particle = GetComponent<ParticleSystem>();

        var main = _particle.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    private void OnParticleSystemStopped()
    {
        PoolManager.Instance.Return(gameObject);
    }

    public void OnSpawn()
    {
        _particle.Play();
    }

    public void OnDespawn()
    {
        _particle.Stop();
        _particle.Clear();
    }
}
