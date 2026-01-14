using System.Collections;
using UnityEngine;
using DG.Tweening;

public class InteractableOutlineController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Outline _outline;
    [SerializeField] private PlayerInteraction _playerInteraction;

    [Header("Settings")]
    [SerializeField] private float _targetWidth = 3f;
    [SerializeField] private float _animationDuration = 0.2f;
    [SerializeField] private Ease _showEase = Ease.OutBack;
    [SerializeField] private Ease _hideEase = Ease.InBack;

    [Header("Throw Cooldown (ItemBase only)")]
    [SerializeField] private float _throwCooldown = 0.5f;

    private IInteractable _interactable;
    private ItemBase _item;
    private Tweener _widthTween;
    private bool _isOutlineActive;
    private bool _isOnCooldown;
    private Coroutine _cooldownCoroutine;

    private void Awake()
    {
        // 컴포넌트 참조 캐싱 (한 번만)
        _interactable = GetComponent<IInteractable>();
        _item = GetComponent<ItemBase>();

        if (_outline == null)
        {
            _outline = GetComponentInChildren<Outline>();
        }

        if (_playerInteraction == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerInteraction = player.GetComponentInChildren<PlayerInteraction>();
            }
        }
    }

    private void OnEnable()
    {
        // 상태 초기화
        _isOutlineActive = false;
        _isOnCooldown = false;
        _cooldownCoroutine = null;

        if (_outline != null)
        {
            _outline.OutlineWidth = 0f;
            _outline.enabled = false;
        }

        // 이벤트 구독
        if (_playerInteraction != null)
        {
            _playerInteraction.OnInteractableChanged += HandleInteractableChanged;
        }

        if (_item != null)
        {
            _item.OnDropped += HandleDropped;
        }
    }

    private void OnDisable()
    {
        // 이벤트 해제
        if (_playerInteraction != null)
        {
            _playerInteraction.OnInteractableChanged -= HandleInteractableChanged;
        }

        if (_item != null)
        {
            _item.OnDropped -= HandleDropped;
        }

        // 정리
        _widthTween?.Kill();
        _widthTween = null;

        if (_cooldownCoroutine != null)
        {
            StopCoroutine(_cooldownCoroutine);
            _cooldownCoroutine = null;
        }
    }

    private void HandleDropped()
    {
        if (_cooldownCoroutine != null)
        {
            StopCoroutine(_cooldownCoroutine);
        }
        _cooldownCoroutine = StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        _isOnCooldown = true;

        if (_isOutlineActive)
        {
            HideOutline();
        }

        yield return new WaitForSeconds(_throwCooldown);

        _isOnCooldown = false;
        _cooldownCoroutine = null;
    }

    private void HandleInteractableChanged(IInteractable interactable)
    {
        if (_isOnCooldown) return;

        bool shouldShow = interactable == _interactable;

        if (shouldShow && !_isOutlineActive)
        {
            ShowOutline();
        }
        else if (!shouldShow && _isOutlineActive)
        {
            HideOutline();
        }
    }

    private void ShowOutline()
    {
        if (_outline == null) return;

        _isOutlineActive = true;
        _widthTween?.Kill();

        _outline.OutlineWidth = 0f;
        _outline.enabled = true;

        _widthTween = DOTween.To(
            () => _outline.OutlineWidth,
            x => _outline.OutlineWidth = x,
            _targetWidth,
            _animationDuration
        ).SetEase(_showEase);
    }

    private void HideOutline()
    {
        if (_outline == null) return;

        _isOutlineActive = false;
        _widthTween?.Kill();

        _widthTween = DOTween.To(
            () => _outline.OutlineWidth,
            x => _outline.OutlineWidth = x,
            0f,
            _animationDuration
        ).SetEase(_hideEase).OnComplete(() =>
        {
            _outline.enabled = false;
        });
    }
}
