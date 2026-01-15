using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_InteractableIcon : MonoBehaviour
{
    [SerializeField] private PlayerInteraction _playerInteraction;
    [SerializeField] private IconData _iconData;
    [SerializeField] private Image _iconImage;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Vector3 _offset = new Vector3(0f, 2f, 0f);

    private Camera _mainCamera;
    private bool _showIcon;
    private Tween _iconChangeTween;
    [SerializeField] private float _iconChangeDuration = 0.2f;
    [SerializeField] private Ease _iconHideEase;
    [SerializeField] private Ease _iconShowEase;
    private void Start()
    {
        _playerInteraction.OnInteractableChanged += HandleInteractableIcon;
        _mainCamera = Camera.main;
    }

    private void OnDestroy()
    {
        _playerInteraction.OnInteractableChanged -= HandleInteractableIcon;
    }

    void LateUpdate()
    {
        if (!_showIcon) return;

        transform.position = _playerTransform.position + _offset;
        transform.rotation = _mainCamera.transform.rotation;
    }

    private void HandleInteractableIcon(IInteractable interactable)
    {
        if (interactable == null || interactable.IconType == IconType.None)
        {
            _iconChangeTween?.Kill();
            _iconChangeTween = _iconImage.gameObject.transform.DOScale(Vector3.zero, _iconChangeDuration).SetEase(_iconHideEase)
                .OnComplete(() => _iconImage.gameObject.SetActive(false));
            _showIcon = false;
            return;
        }

        _showIcon = true;
        _iconImage.gameObject.SetActive(true);
        _iconImage.sprite = _iconData.GetIcon(interactable.IconType);
        _iconChangeTween?.Kill();
        _iconChangeTween = _iconImage.gameObject.transform.DOScale(Vector3.one, _iconChangeDuration).SetEase(_iconShowEase);
    }
}
