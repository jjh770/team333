using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MonsterDamageComponent : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float _damageDuration = 0.09f;

    private bool _isDamaged;
    public bool IsDamaged => _isDamaged;

    [Header("Hit Effect")]
    [SerializeField] private SkinnedMeshRenderer _renderer;
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private float _flashDuration = 0.3f;

    [Header("Knockback")]
    [SerializeField] private float _knockbackForce = 2.0f;
    [SerializeField] private float _knockbackDuration = 0.2f;

    private Material _material;
    private Tweener _flashTween;
    private Tweener _knockbackTween;

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

    public void ApplyKnockback(Vector3 attackerPosition)
    {
        Vector3 direction = (transform.position - attackerPosition).normalized;
        direction.y = 0;
        Vector3 knockbackTarget = transform.position + (direction * _knockbackForce);

        _knockbackTween?.Kill();

        // 뒤로 밀려나기
        _knockbackTween = transform.DOMove(knockbackTarget, _knockbackDuration)
                    .SetEase(Ease.OutQuad);
    }
}
