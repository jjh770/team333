using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_MonsterSpeechBubble : MonoBehaviour
{
    [Header("SpeechBubble")]
    [SerializeField] private GameObject _speechBubble;
    [SerializeField] private TextMeshProUGUI _text;

    [Header("text")]
    [SerializeField] private string _happyText = "Take me";
    [SerializeField] private string _sadText = "Help me";
    private string _currrentText;

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
        SetText(_sadText);
    }

    public void SetHappy()
    {
        SetText(_happyText);
    }

    public void Hide()
    {
        _speechBubble.SetActive(false);
    }

    private void SetText(string newText)
    {
        _currrentText = newText;
        _text.text = _currrentText;

        PlayPopAnimation();
    }

    private void PlayPopAnimation()
    {
        _iconTween?.Kill(true);

        _text.transform.localScale = Vector3.one;
        _iconTween = _text.transform.DOScale(_popScale, _popDuration)
            .SetLoops(2, LoopType.Yoyo);
    }

    private void OnDestroy()
    {
        _floatTween?.Kill();
        _iconTween?.Kill();
    }
}