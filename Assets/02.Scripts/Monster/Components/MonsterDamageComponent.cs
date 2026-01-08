using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class MonsterDamageComponent : MonoBehaviour, IDamageable
{
    [Header("Damage")]
    [SerializeField] private float _damageDuration = 0.09f;

    private MonsterStat _monsterStat;
    protected NavMeshAgent _agent;
    private bool _isDamaged;
    public bool IsDamaged => _isDamaged;

    [Header("Hit Effect")]
    [SerializeField] private SkinnedMeshRenderer _renderer;
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private float _flashDuration = 0.3f;

    private Material _material;
    private Tweener _flashTween;

    private static readonly int s_emissionColor = Shader.PropertyToID("_Emissive_Color");

    private void Awake()
    {
        _monsterStat = GetComponent<MonsterStat>();
        _agent = GetComponent<NavMeshAgent>();

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Damage damage = new Damage(30f, null);
            TryTakeDamage(damage);
        }
    }

    public bool TryTakeDamage(Damage damage)
    {
        if (damage.Value <= 0)
            return false;

        _monsterStat.DecreaseHealth(damage.Value);
        FlashWhite();

        if (!_isDamaged)
        {
            StartCoroutine(Damage_Coroutine());
        }

        return true;
    }

    private void FlashWhite()
    {
        if (_material == null) return;

        _flashTween?.Kill();
        _material.SetColor(s_emissionColor, _flashColor);
        _flashTween = _material.DOColor(Color.black, s_emissionColor, _flashDuration);
    }

    private IEnumerator Damage_Coroutine()
    {
        _isDamaged = true;
        yield return new WaitForSeconds(_damageDuration);
        _isDamaged = false;
    }
}
