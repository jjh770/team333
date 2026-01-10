using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_MonsterSpeechBubble : MonoBehaviour
{
    [SerializeField] private GameObject _speechBubble;
    [SerializeField] private Image _iconImage;

    [Header("Sprites")]
    [SerializeField] private Sprite _sadSprite;
    [SerializeField] private Sprite _happySprite;
    private Sprite _currrentSprite;

    [Header("Animation")]
    [SerializeField] private float _fadeDuration = 0.12f;
    [SerializeField] private float _popScale = 1.08f;
    [SerializeField] private float _popDuration = 0.12f;

    private Tweener _tween;

    public void SetSad()
    {
        SetIcon(_sadSprite);
    }

    public void SetHappy()
    {
        SetIcon(_happySprite);
    }

    private void SetIcon(Sprite sprite)
    {
        _currrentSprite = sprite;
        _iconImage.sprite = _currrentSprite;
    }
}
