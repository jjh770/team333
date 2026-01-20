using DG.Tweening;
using System.Collections;
using UnityEngine;

public abstract class MonsterMoveComponent : MonoBehaviour
{
    [SerializeField] protected float _velocityThreshold = 0.1f;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 5f;
    private const float MinLookDirectionSqrMagnitude = 1e-6f;

    [Header("Knockback")]
    [SerializeField] private float _knockbackForce = 2f;
    [SerializeField] private float _knockbackDuration = 0.2f;
    [SerializeField] private float _knockbackStunDuration = 0.3f;
    private Tweener _knockbackTween;

    protected Transform _target;

    protected bool _isMoving;
    public bool IsMoving => _isMoving;

    protected bool _isKnockbackStunned;
    public bool IsKnockbackStunned => _isKnockbackStunned;

    protected bool _isStunned;
    public bool IsStunned => _isStunned;

    public virtual bool CanKnockback => true;

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public abstract void UpdateMove();

    protected virtual void OnDisable()
    {
        _knockbackTween?.Kill();
        _isKnockbackStunned = false;
    }

    public virtual void Enable() { }
    public virtual void Disable() { }
    public virtual void Stop() { }
    public virtual void Resume() { }
    public virtual void SetSpeed(float speed) { }
    public virtual void ResetMove() { }

    protected void LookAtTarget()
    {
        if (_target == null) return;

        Vector3 direction = _target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > MinLookDirectionSqrMagnitude)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }

    public void LookAtTargetImmediate()
    {
        if (_target == null) return;

        Vector3 direction = _target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > MinLookDirectionSqrMagnitude)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public IEnumerator StunCoroutine(float duration)
    {
        _isStunned = true;

        Stop();

        yield return new WaitForSeconds(duration);

        _isStunned = false;
        Resume();
    }

    public void ApplyKnockback(Vector3 attackerPosition)
    {
        if (!CanKnockback) return;

        Vector3 direction = (transform.position - attackerPosition).normalized;
        direction.y = 0;
        Vector3 knockbackTarget = transform.position + (direction * _knockbackForce);

        _knockbackTween?.Kill();
        _isKnockbackStunned = true;

        // 뒤로 밀려나기
        _knockbackTween = transform.DOMove(knockbackTarget, _knockbackDuration)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        if (gameObject.activeInHierarchy)
                        {
                            StartCoroutine(KnockbackStunCoroutine());
                        }
                    });
    }

    private IEnumerator KnockbackStunCoroutine()
    {
        yield return new WaitForSeconds(_knockbackStunDuration);
        _isKnockbackStunned = false;
    }
}
