using DG.Tweening;
using GameUI.Animations;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SpriteAnimation
{
    public Image TargetImage;
    public Sprite[] Sprites;
    public float FrameRate = 2f;

    private int _currentFrame;
    private float _timer;

    private float FrameInterval => 1f / FrameRate;

    public void Reset()
    {
        _currentFrame = 0;
        _timer = 0f;
        UpdateSprite();
    }

    public bool Update(float deltaTime, bool loop)
    {
        _timer += deltaTime;

        if (_timer >= FrameInterval)
        {
            _timer -= FrameInterval;
            return NextFrame(loop);
        }

        return true;
    }

    private bool NextFrame(bool loop)
    {
        _currentFrame++;

        if (_currentFrame >= Sprites.Length)
        {
            if (loop)
            {
                _currentFrame = 0;
            }
            else
            {
                _currentFrame = Sprites.Length - 1;
                return false;
            }
        }

        UpdateSprite();
        return true;
    }

    public void UpdateSprite()
    {
        if (TargetImage != null && Sprites != null && _currentFrame < Sprites.Length)
        {
            TargetImage.sprite = Sprites[_currentFrame];
        }
    }
}

public class UI_LoadingAnimation : MonoBehaviour
{
    [Header("Animations")]
    [SerializeField] private SpriteAnimation[] _animations;

    [Header("TextAnimations")]
    [SerializeField] private TextMeshProUGUI _loadingText;
    [SerializeField] private float _textSizeMultiflier = 1.2f;
    [SerializeField] private float _textDuration = 1.2f;

    [Header("Settings")]
    [SerializeField] private bool _loop = true;
    [SerializeField] private bool _playOnAwake = true;

    [Header("Position Animations")]
    [SerializeField] private List<UIElementDelayAnimation> _delayedPositionAnimations;

    private bool _isPlaying;

    private void Awake()
    {
        foreach (var anim in _delayedPositionAnimations)
        {
            anim.SetToHidden();
        }
    }
    private void Start()
    {
        if (_playOnAwake)
        {
            Play();
        }
        LoadingTextAnimation();
    }

    private void Update()
    {
        if (!_isPlaying || _animations == null || _animations.Length == 0) return;

        bool anyPlaying = false;
        float deltaTime = Time.unscaledDeltaTime;

        foreach (var anim in _animations)
        {
            if (anim.Update(deltaTime, _loop))
            {
                anyPlaying = true;
            }
        }

        if (!anyPlaying && !_loop)
        {
            _isPlaying = false;
        }
    }

    public void Play()
    {
        _isPlaying = true;

        foreach (var anim in _animations)
        {
            anim.Reset();
        }

        foreach (var anim in _delayedPositionAnimations)
        {
            anim.AnimateToVisible();
        }
    }

    private void LoadingTextAnimation()
    {
        _loadingText.transform.DOScale(Vector3.one * _textSizeMultiflier, _textDuration).SetLoops(-1, LoopType.Yoyo);
    }
}
