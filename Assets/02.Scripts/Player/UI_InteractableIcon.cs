using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_InteractableIcon : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private PlayerInteraction _playerInteraction;
    [SerializeField] private PlayerPickUpThrow _playerPickUpThrow;
    [SerializeField] private IconData _iconData;

    [Header("Interactable")]
    [SerializeField] private Image _interactableIconContainer;
    [SerializeField] private Image _interactableIconImage;

    [Header("PickUpObject")]
    [SerializeField] private GameObject _pickUpIconContainer;
    [SerializeField] private Image[] _pickUpIconImages = new Image[2];

    [Header("Position")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Vector3 _offset = new Vector3(0f, 1f, 0f);

    [Header("Animation")]
    [SerializeField] private float _iconChangeDuration = 0.2f;
    [SerializeField] private Ease _iconHideEase;
    [SerializeField] private Ease _iconShowEase;

    private Camera _mainCamera;

    // Interactable 아이콘 상태
    private Tween _interactableTween;
    private bool _isInteractableShowing;
    private IconType _currentInteractableIconType;

    // PickUp 컨테이너 상태
    private Tween _pickUpContainerTween;
    private bool _isPickUpContainerShowing;

    // PickUp 아이콘 상태
    private Tween[] _pickUpIconTweens;
    private bool[] _isPickUpIconShowing;
    private IconType[] _currentPickUpIconTypes;

    private IInteractable _currentInteractable;
    private List<IPickable> _heldItems = new List<IPickable>();

    private void Start()
    {
        _playerInteraction.OnInteractableChanged += HandleInteractableChanged;
        _playerPickUpThrow.OnPickedUpItem += HandlePickedUpItem;
        _playerPickUpThrow.OnThrownItem += HandleThrownItem;
        _mainCamera = Camera.main;

        // 배열 초기화
        int slotCount = _pickUpIconImages.Length;
        _pickUpIconTweens = new Tween[slotCount];
        _isPickUpIconShowing = new bool[slotCount];
        _currentPickUpIconTypes = new IconType[slotCount];

        InitializeUI();
    }

    private void OnDestroy()
    {
        _playerInteraction.OnInteractableChanged -= HandleInteractableChanged;
        _playerPickUpThrow.OnPickedUpItem -= HandlePickedUpItem;
        _playerPickUpThrow.OnThrownItem -= HandleThrownItem;

        // Tween 정리
        _interactableTween?.Kill();
        _pickUpContainerTween?.Kill();
        if (_pickUpIconTweens != null)
        {
            foreach (var tween in _pickUpIconTweens)
            {
                tween?.Kill();
            }
        }
    }

    private void LateUpdate()
    {
        if (!HasAnyIconShowing()) return;

        transform.position = _playerTransform.position + _offset;
        transform.rotation = _mainCamera.transform.rotation;
    }

    private bool HasAnyIconShowing()
    {
        return _isInteractableShowing || _isPickUpContainerShowing;
    }

    private void HandleInteractableChanged(IInteractable interactable)
    {
        _currentInteractable = interactable;
        UpdateInteractableIcon();
    }

    private void HandlePickedUpItem(IPickable pickable)
    {
        if (pickable != null && !_heldItems.Contains(pickable))
        {
            _heldItems.Add(pickable);
        }
        UpdatePickUpIcons();
    }

    private void HandleThrownItem(IPickable pickable)
    {
        _heldItems.Remove(pickable);
        UpdatePickUpIcons();
    }

    private void UpdateInteractableIcon()
    {
        if (_currentInteractable != null && !IsDestroyed(_currentInteractable) && _currentInteractable.IconType != IconType.None)
        {
            ShowInteractableIcon(_currentInteractable.IconType);
        }
        else
        {
            HideInteractableIcon();
        }
    }

    private void UpdatePickUpIcons()
    {
        // 파괴된 아이템 제거
        _heldItems.RemoveAll(item => item == null || IsDestroyed(item));

        bool hasAnyItem = false;

        for (int i = 0; i < _pickUpIconImages.Length; i++)
        {
            if (_pickUpIconImages[i] == null) continue;

            if (i < _heldItems.Count && _heldItems[i] != null && _heldItems[i].IconType != IconType.None)
            {
                ShowPickUpIcon(i, _heldItems[i].IconType);
                hasAnyItem = true;
            }
            else
            {
                HidePickUpIcon(i);
            }
        }

        // 컨테이너 표시/숨김
        if (hasAnyItem)
        {
            ShowPickUpContainer();
        }
        else
        {
            HidePickUpContainer();
        }
    }

    private void ShowInteractableIcon(IconType iconType)
    {
        // 이미 같은 아이콘이 표시 중이면 무시
        if (_isInteractableShowing && _currentInteractableIconType == iconType) return;

        _isInteractableShowing = true;
        _currentInteractableIconType = iconType;

        _interactableIconContainer.gameObject.SetActive(true);
        _interactableIconImage.sprite = _iconData.GetIcon(iconType);

        _interactableTween?.Kill();
        _interactableTween = _interactableIconContainer.transform
            .DOScale(Vector3.one, _iconChangeDuration)
            .SetEase(_iconShowEase);
    }

    private void HideInteractableIcon()
    {
        if (!_isInteractableShowing) return;

        _isInteractableShowing = false;
        _currentInteractableIconType = IconType.None;

        _interactableTween?.Kill();
        _interactableTween = _interactableIconContainer.transform
            .DOScale(Vector3.zero, _iconChangeDuration)
            .SetEase(_iconHideEase)
            .OnComplete(() => _interactableIconContainer.gameObject.SetActive(false));
    }

    private void ShowPickUpContainer()
    {
        if (_isPickUpContainerShowing) return;

        _isPickUpContainerShowing = true;
        _pickUpIconContainer.SetActive(true);

        _pickUpContainerTween?.Kill();
        _pickUpContainerTween = _pickUpIconContainer.transform
            .DOScale(Vector3.one, _iconChangeDuration)
            .SetEase(_iconShowEase);
    }

    private void HidePickUpContainer()
    {
        if (!_isPickUpContainerShowing) return;

        _isPickUpContainerShowing = false;

        _pickUpContainerTween?.Kill();
        _pickUpContainerTween = _pickUpIconContainer.transform
            .DOScale(Vector3.zero, _iconChangeDuration)
            .SetEase(_iconHideEase)
            .OnComplete(() => _pickUpIconContainer.SetActive(false));
    }

    private void ShowPickUpIcon(int index, IconType iconType)
    {
        // 이미 같은 아이콘이 표시 중이면 무시
        if (_isPickUpIconShowing[index] && _currentPickUpIconTypes[index] == iconType) return;

        _isPickUpIconShowing[index] = true;
        _currentPickUpIconTypes[index] = iconType;

        var iconImage = _pickUpIconImages[index];
        iconImage.sprite = _iconData.GetIcon(iconType);
        iconImage.gameObject.SetActive(true);

        _pickUpIconTweens[index]?.Kill();
        _pickUpIconTweens[index] = iconImage.transform
            .DOScale(Vector3.one, _iconChangeDuration)
            .SetEase(_iconShowEase);
    }

    private void HidePickUpIcon(int index)
    {
        if (!_isPickUpIconShowing[index]) return;

        _isPickUpIconShowing[index] = false;
        _currentPickUpIconTypes[index] = IconType.None;

        var iconImage = _pickUpIconImages[index];

        _pickUpIconTweens[index]?.Kill();
        _pickUpIconTweens[index] = iconImage.transform
            .DOScale(Vector3.zero, _iconChangeDuration)
            .SetEase(_iconHideEase)
            .OnComplete(() => iconImage.gameObject.SetActive(false));
    }

    private void InitializeUI()
    {
        // 초기 상태 설정 (애니메이션 없이)
        _interactableIconContainer.transform.localScale = Vector3.zero;
        _interactableIconContainer.gameObject.SetActive(false);

        _pickUpIconContainer.transform.localScale = Vector3.zero;
        _pickUpIconContainer.SetActive(false);

        foreach (var iconImage in _pickUpIconImages)
        {
            if (iconImage == null) continue;
            iconImage.transform.localScale = Vector3.zero;
            iconImage.gameObject.SetActive(false);
        }
    }

    private bool IsDestroyed(object obj)
    {
        if (obj is Object unityObj)
        {
            return unityObj == null;
        }
        return obj == null;
    }
}
