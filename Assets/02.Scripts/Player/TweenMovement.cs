using System;
using DG.Tweening;
using UnityEngine;

public class TweenMovement : MonoBehaviour
{
    private CharacterController _controller;
    private PlayerMove _playerMove;
    private Camera _mainCamera;
    private Tweener _currentTween;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _playerMove = GetComponent<PlayerMove>();
        _mainCamera = Camera.main;
    }

    private void OnDestroy()
    {
        _currentTween?.Kill();
    }

    /// <summary>
    /// 선형 이동 (공격, 대시용)
    /// </summary>
    public void StartLinearMovement(Vector3 direction, float distance, float duration, Ease ease, Action onComplete = null)
    {
        if (_controller == null) return;

        _currentTween?.Kill();

        float previousValue = 0f;

        _currentTween = DOVirtual.Float(0f, distance, duration, (currentValue) =>
        {
            float delta = currentValue - previousValue;
            Vector3 moveVector = direction * delta;

            Vector3 clampedMove = CameraBoundsHelper.ClampMovementToCameraBounds(
                transform.position, moveVector, _mainCamera, _playerMove.ViewportMargin);

            _controller.Move(clampedMove);

            previousValue = currentValue;
        })
        .SetEase(ease)
        .OnComplete(() => onComplete?.Invoke())
        .OnKill(() => _currentTween = null);
    }

    /// <summary>
    /// 포물선 이동 (스킬용)
    /// </summary>
    public void StartParabolicMovement(Vector3 direction, float distance, float jumpHeight, float duration, Ease ease)
    {
        if (_controller == null) return;

        _currentTween?.Kill();

        float prevHorizontal = 0f;
        float prevVertical = 0f;

        _currentTween = DOVirtual.Float(0f, 1f, duration, (t) =>
        {
            float currentHorizontal = t * distance;
            float deltaHorizontal = currentHorizontal - prevHorizontal;

            // 포물선: 4 * h * t * (1 - t)
            float currentVertical = 4 * jumpHeight * t * (1 - t);
            float deltaVertical = currentVertical - prevVertical;

            Vector3 moveVector = (direction * deltaHorizontal) + (Vector3.up * deltaVertical);

            Vector3 clampedMove = CameraBoundsHelper.ClampMovementToCameraBounds(
                transform.position, moveVector, _mainCamera, _playerMove.ViewportMargin);

            _controller.Move(clampedMove);

            prevHorizontal = currentHorizontal;
            prevVertical = currentVertical;
        })
        .SetEase(ease)
        .OnKill(() => _currentTween = null);
    }

    /// <summary>
    /// 이동 중단
    /// </summary>
    public void Stop()
    {
        if (_currentTween != null && _currentTween.IsActive())
        {
            _currentTween.Kill();
        }
    }

    public bool IsMoving => _currentTween != null && _currentTween.IsActive();
}
