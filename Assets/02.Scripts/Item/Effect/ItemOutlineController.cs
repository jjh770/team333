using System.Collections;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(ItemBase))]
public class ItemOutlineController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Outline _outline;
    [SerializeField] private PlayerInteraction _playerInteraction;

    [Header("Settings")]
    [SerializeField] private float _targetWidth = 3f;
    [SerializeField] private float _animationDuration = 0.3f;
    [SerializeField] private Ease _showEase = Ease.OutBack;
    [SerializeField] private Ease _hideEase = Ease.InBack;
    [SerializeField] private float _throwCooldown = 0.5f;

    private ItemBase _item;
    private Tweener _widthTween;
    private bool _isOutlineActive;
    private bool _isOnCooldown;

    private void Awake()
    {
        _item = GetComponent<ItemBase>();

        if (_outline == null)
        {
            _outline = GetComponentInChildren<Outline>();
        }
    }

    private void Start()
    {
        if (_outline != null)
        {
            _outline.OutlineWidth = 0f;
            _outline.enabled = false;
        }

        if (_playerInteraction == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerInteraction = player.GetComponentInChildren<PlayerInteraction>();
            }
        }

        if (_playerInteraction != null)
        {
            _playerInteraction.OnInteractableChanged += HandleInteractableChanged;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(WatchHeldState());
    }

    private IEnumerator WatchHeldState()
    {
        while (true)
        {
            // 들릴 때까지 대기
            yield return new WaitUntil(() => _item.IsHeld);

            // 놓을 때까지 대기
            yield return new WaitUntil(() => !_item.IsHeld);

            // 던졌으므로 쿨다운 시작
            _isOnCooldown = true;
            if (_isOutlineActive)
            {
                HideOutline();
            }

            yield return new WaitForSeconds(_throwCooldown);
            _isOnCooldown = false;
        }
    }

    private void OnDestroy()
    {
        if (_playerInteraction != null)
        {
            _playerInteraction.OnInteractableChanged -= HandleInteractableChanged;
        }

        _widthTween?.Kill();
    }

    private void HandleInteractableChanged(IInteractable interactable)
    {
        // 쿨다운 중이면 무시
        if (_isOnCooldown) return;

        bool shouldShow = interactable == _item as IInteractable;

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
