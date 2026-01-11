using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_MonsterSpeechBubble : MonoBehaviour
{
    [Header("SpeechBubble")]
    [SerializeField] private GameObject _speechBubble;
    [SerializeField] private Image _iconImage;

    [Header("Sprites")]
    [SerializeField] private Sprite _sadSprite;
    [SerializeField] private Sprite _happySprite;
    private Sprite _currrentSprite;

    [Header("Icon Animation")]
    [SerializeField] private float _popScale = 1.08f;
    [SerializeField] private float _popDuration = 0.12f;

    [Header("Floating Animation")]
    [SerializeField] private float _floatDistance = 0.1f;
    [SerializeField] private float _floatDuration = 1.5f;

    private Tween _floatTween;
    private Tweener _iconTween;

    private void Start()
    {
        StartFloating();
    }

    private void StartFloating()
    {
        
        RectTransform bubbleRect = _speechBubble.GetComponent<RectTransform>();

        _floatTween = bubbleRect.DOAnchorPosY(bubbleRect.anchoredPosition.y + _floatDistance, _floatDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void SetSad()
    {
        SetIcon(_sadSprite);
    }

    public void SetHappy()
    {
        SetIcon(_happySprite);
    }

    public void Hide()
    {
        _speechBubble.SetActive(false);
    }

    private void SetIcon(Sprite sprite)
    {
        _currrentSprite = sprite;
        _iconImage.sprite = _currrentSprite;

        PlayPopAnimation();
    }

    private void PlayPopAnimation()
    {
        _iconTween?.Kill(true);

        _iconImage.transform.localScale = Vector3.one;
        _iconTween = _iconImage.transform.DOScale(_popScale, _popDuration)
            .SetLoops(2, LoopType.Yoyo);
    }

    private void OnDestroy()
    {
        _floatTween?.Kill();
        _iconTween?.Kill();
    }
}