using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_MonsterSpeechBubble : MonoBehaviour
{
    [Header("SpeechBubble")]
    [SerializeField] private GameObject _speechBubble;
    [SerializeField] private Image _speechBubbleImage;

    [Header("Icon")]
    [SerializeField] private Sprite _defaultIcon;
    [SerializeField] private Sprite _happyIcon;
    [SerializeField] private Sprite _sadIcon;
    private Sprite _currentIcon;

    [Header("Icon Animation")]
    [SerializeField] private float _popScale = 1.08f;
    [SerializeField] private float _popDuration = 0.12f;

    [Header("Floating Animation")]
    [SerializeField] private float _floatDistance = 0.5f;
    [SerializeField] private float _floatDuration = 1.5f;

    private Tween _floatTween;
    private Tweener _iconTween;

    private void Start()
    {
        StartFloating();
        SetDefaultIcon();
    }

    private void StartFloating()
    {
        RectTransform bubbleRect = _speechBubble.GetComponent<RectTransform>();

        _floatTween = bubbleRect.DOAnchorPosY(bubbleRect.anchoredPosition.y + _floatDistance, _floatDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void SetDefaultIcon()
    {
        if (_defaultIcon == null) return;
        SetIcon(_defaultIcon);
    }

    public void SetSad()
    {
        SetIcon(_sadIcon);
    }

    public void SetHappy()
    {
        SetIcon(_happyIcon);
    }

    public void Hide()
    {
        _speechBubble.SetActive(false);
    }

    private void SetIcon(Sprite newIcon)
    {
        if (_speechBubbleImage == null) return;

        _currentIcon = newIcon;
        _speechBubbleImage.sprite = _currentIcon;

        PlayPopAnimation();
    }

    private void PlayPopAnimation()
    {
        if (_speechBubbleImage == null) return;

        _iconTween?.Kill(true);

        _speechBubbleImage.transform.localScale = Vector3.one;
        _iconTween = _speechBubbleImage.transform.DOScale(_popScale, _popDuration)
            .SetLoops(2, LoopType.Yoyo);
    }

    private void OnDestroy()
    {
        _floatTween?.Kill();
        _iconTween?.Kill();
    }
}