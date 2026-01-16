using GameUI.Animations;
using System.Collections.Generic;
using UnityEngine;

public class UI_ContainerByState : MonoBehaviour
{
    [Header("즉시 애니메이션")]
    [SerializeField] private List<UIElementAnimation> _immediatePositionAnimations;

    [Header("지연 애니메이션")]
    [SerializeField] private List<UIElementDelayAnimation> _delayedPositionAnimations;

    [Header("페이드 애니메이션")]
    [SerializeField] private List<UIFadeAnimation> _fadeAnimations;

    private void Awake()
    {
        foreach (var anim in _immediatePositionAnimations)
        {
            anim.SetToHidden();
        }

        foreach (var anim in _delayedPositionAnimations)
        {
            anim.SetToHidden();
        }

        foreach (var fade in _fadeAnimations)
        {
            fade.SetToHidden();
        }
    }

    private void Start()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnStateChanged += HandleGameStateChanged;
        }
    }

    private void OnDestroy()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnStateChanged -= HandleGameStateChanged;
        }
    }

    private void HandleGameStateChanged(GameState oldState, GameState newState)
    {
        if (oldState == GameState.Intro && newState == GameState.Playing)
        {
            foreach (var anim in _immediatePositionAnimations)
            {
                anim.AnimateToVisible();
            }

            foreach (var anim in _delayedPositionAnimations)
            {
                anim.AnimateToVisible();
            }

            foreach (var fade in _fadeAnimations)
            {
                fade.AnimateToVisible();
            }
        }
        else if (oldState == GameState.Playing && newState == GameState.Outro)
        {
            foreach (var anim in _immediatePositionAnimations)
            {
                anim.AnimateToHidden();
            }

            foreach (var anim in _delayedPositionAnimations)
            {
                anim.AnimateToHidden();
            }

            foreach (var fade in _fadeAnimations)
            {
                fade.AnimateToHidden();
            }
        }
    }
}
