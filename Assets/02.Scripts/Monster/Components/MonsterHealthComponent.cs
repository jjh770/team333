using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MonsterHealthComponent : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float _damageDuration = 0.09f;

    [Header("Hit Effect")]
    [SerializeField] private SkinnedMeshRenderer _renderer;
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private float _flashDuration = 0.3f;

    private Material _material;
    private Tweener _flashTween;
    private Tweener _knockbackTween;

    private bool _isKnockbackStunned;
    public bool IsKnockbackStunned => _isKnockbackStunned;

    private bool _isDamaged;
    public bool IsDamaged => _isDamaged;

    protected bool _isStunned;
    public bool IsStunned => _isStunned;

    private static readonly int s_emissionColor = Shader.PropertyToID("_Emissive_Color");

    private void Awake()
    {
        if (_renderer != null)
        {
            _material = _renderer.material;
        }
    }

    private void OnEnable()
    {
        _isDamaged = false;
        _isKnockbackStunned = false;
    }

    private void OnDisable()
    {
        _flashTween?.Kill();
        _knockbackTween?.Kill();

        if (_material != null)
        {
            _material.SetColor(s_emissionColor, Color.black);
        }
    }

    public IEnumerator DamageCoroutine()
    {
        _isDamaged = true;
        yield return new WaitForSeconds(_damageDuration);
        _isDamaged = false;
    }

    public void FlashWhite()
    {
        if (_material == null) return;

        _flashTween?.Kill();
        _material.SetColor(s_emissionColor, _flashColor);
        _flashTween = _material.DOColor(Color.black, s_emissionColor, _flashDuration);
    }


}
