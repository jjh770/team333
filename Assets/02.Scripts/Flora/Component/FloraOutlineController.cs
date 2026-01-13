using UnityEngine;
using DG.Tweening;

public class FloraOutlineController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Outline _outline;
    [SerializeField] private PlayerPickUpThrow _playerPickUpThrow;

    [Header("Settings")]
    [SerializeField] private float _targetWidth = 3f;
    [SerializeField] private float _animationDuration = 0.5f;
    [SerializeField] private Ease _easeType = Ease.OutBack;

    private Tweener _widthTween;

    private void Start()
    {
        if (_outline == null)
        {
            _outline = GetComponentInChildren<Outline>();
        }

        if (_outline != null)
        {
            _outline.OutlineWidth = 0f;
            _outline.enabled = false;
        }

        if (_playerPickUpThrow != null)
        {
            _playerPickUpThrow.OnHoldingChanged += HandleHoldingChanged;
        }
    }

    private void OnDestroy()
    {
        if (_playerPickUpThrow != null)
        {
            _playerPickUpThrow.OnHoldingChanged -= HandleHoldingChanged;
        }

        _widthTween?.Kill();
    }

    private void HandleHoldingChanged(bool isHolding)
    {
        if (_outline == null) return;

        _widthTween?.Kill();

        if (isHolding)
        {
            _outline.OutlineWidth = 0f;
            _outline.enabled = true;

            _widthTween = DOTween.To(
                () => _outline.OutlineWidth,
                x => _outline.OutlineWidth = x,
                _targetWidth,
                _animationDuration
            ).SetEase(_easeType);
        }
        else
        {
            _widthTween = DOTween.To(
                () => _outline.OutlineWidth,
                x => _outline.OutlineWidth = x,
                0f,
                _animationDuration
            ).SetEase(Ease.InBack).OnComplete(() =>
            {
                _outline.enabled = false;
            });
        }
    }
}
