using System.Collections;
using UnityEngine;

public class PlayerDamageComponent : MonoBehaviour, IDamageable
{
    [Header("Damage Settings")]
    [SerializeField] private float _damageDuration = 0.09f;
    [SerializeField] private float _invincibilityDuration = 1f; // 무적 시간

    [Header("Visual Feedback")]
    [SerializeField] private bool _enableFlashing = true;
    [SerializeField] private float _flashInterval = 0.1f;

    private PlayerStat _playerStat;
    private PlayerStateManager _stateManager;
    private bool _isDamaged;
    private bool _isInvincible;
    private Renderer[] _renderers;

    public bool IsDamaged => _isDamaged;
    public bool IsInvincible => _isInvincible;

    private void Awake()
    {
        _playerStat = GetComponent<PlayerStat>();
        _stateManager = GetComponent<PlayerStateManager>();
        _renderers = GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Damage damage = new Damage(30f, null);
            TryTakeDamage(damage);
        }
    }

    public bool TryTakeDamage(Damage damage)
    {
        if (_stateManager.IsState(PlayerState.Die))
            return false;

        // 무적 상태면 데미지 무시
        if (_isInvincible)
            return false;

        if (damage.Value <= 0)
            return false;

        _playerStat.DecreaseHealth(damage.Value);

        if (!_isDamaged)
        {
            StartCoroutine(Damage_Coroutine());
        }
        return true;
    }

    private IEnumerator Damage_Coroutine()
    {
        _isDamaged = true;

        // 피격 이펙트 시간
        yield return new WaitForSeconds(_damageDuration);

        _isDamaged = false;

        // 무적 시간 시작
        StartCoroutine(Invincibility_Coroutine());
    }

    /// <summary>
    /// 무적 시간 처리 코루틴
    /// </summary>
    private IEnumerator Invincibility_Coroutine()
    {
        _isInvincible = true;
        float elapsedTime = 0f;

        // 깜빡임 효과
        if (_enableFlashing)
        {
            while (elapsedTime < _invincibilityDuration)
            {
                // 렌더러 끄기
                SetRenderersEnabled(false);
                yield return new WaitForSeconds(_flashInterval);

                SetRenderersEnabled(true);
                yield return new WaitForSeconds(_flashInterval);

                elapsedTime += _flashInterval * 2f;
            }

            // 무적 종료 시 렌더러 켜기
            SetRenderersEnabled(true);
        }
        else
        {
            // 깜빡임 없이 무적 시간만 대기
            yield return new WaitForSeconds(_invincibilityDuration);
        }

        _isInvincible = false;
    }

    /// <summary>
    /// 모든 렌더러 활성화/비활성화
    /// </summary>
    private void SetRenderersEnabled(bool enabled)
    {
        foreach (var renderer in _renderers)
        {
            if (renderer != null)
            {
                renderer.enabled = enabled;
            }
        }
    }

    /// <summary>
    /// 외부에서 무적 상태를 강제로 설정/해제
    /// </summary>
    public void SetInvincible(bool invincible)
    {
        if (invincible)
        {
            if (!_isInvincible)
            {
                StopAllCoroutines();
                StartCoroutine(Invincibility_Coroutine());
            }
        }
        else
        {
            StopAllCoroutines();
            _isInvincible = false;
            _isDamaged = false;
            SetRenderersEnabled(true);
        }
    }
}
