using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(FloraInteraction))]
public class FloraOutlineController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Outline _outline;
    [SerializeField] private PlayerInteraction _playerInteraction;
    [SerializeField] private PlayerPickUpThrow _playerPickUpThrow;

    [Header("Settings")]
    [SerializeField] private float _targetWidth = 3f;
    [SerializeField] private float _animationDuration = 0.5f;
    [SerializeField] private Ease _showEase = Ease.OutBack;
    [SerializeField] private Ease _hideEase = Ease.InBack;

    private FloraInteraction _floraInteraction;
    private Tweener _widthTween;

    private bool _isPlayerHolding;
    private bool _canTalkToFlora;
    private bool _isOutlineActive;

    private bool ShouldShowOutline => _isPlayerHolding || _canTalkToFlora;

    private void Start()
    {
        _floraInteraction = GetComponent<FloraInteraction>();

        if (_outline == null)
        {
            _outline = GetComponentInChildren<Outline>();
        }

        if (_outline != null)
        {
            _outline.OutlineWidth = 0f;
            _outline.enabled = false;
        }

        if (_playerInteraction != null)
        {
            _playerInteraction.OnInteractableChanged += HandleInteractableChanged;
        }

        if (_playerPickUpThrow != null)
        {
            _playerPickUpThrow.OnHoldingChanged += HandleHoldingChanged;
        }
    }

    private void OnDestroy()
    {
        if (_playerInteraction != null)
        {
            _playerInteraction.OnInteractableChanged -= HandleInteractableChanged;
        }

        if (_playerPickUpThrow != null)
        {
            _playerPickUpThrow.OnHoldingChanged -= HandleHoldingChanged;
        }

        _widthTween?.Kill();
    }

    private void HandleInteractableChanged(IInteractable interactable)
    {
        _canTalkToFlora = ReferenceEquals(interactable, _floraInteraction);
        UpdateOutline();
    }

    private void HandleHoldingChanged(bool isHolding)
    {
        _isPlayerHolding = isHolding;
        UpdateOutline();
    }

    private void UpdateOutline()
    {
        if (ShouldShowOutline && !_isOutlineActive)
        {
            ShowOutline();
        }
        else if (!ShouldShowOutline && _isOutlineActive)
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
