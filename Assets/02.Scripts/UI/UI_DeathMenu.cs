using DG.Tweening;
using GameUI.Animations;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_DeathMenu : MonoBehaviour
{
    [Header("UI Panel")]
    [SerializeField] private GameObject _deathPanel;

    [SerializeField] private UIFadeAnimation _backgroundFade;
    [SerializeField] private UIElementDelayAnimation _delayedPanelAnimation;
    [SerializeField] private UIElementDelayAnimation _ropeAnimation;

    [Header("Buttons")]
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _lobbyButton;

    [Header("Rope Swing Settings")]
    [SerializeField] private GameObject _rope;
    [SerializeField] private float _swingAngle = 15f;
    [SerializeField] private float _swingDuration = 1f;
    [SerializeField] private Ease _swingEase = Ease.InOutSine;

    private Tween _ropeTween;

    public static event Action OnRestartRequested;
    public static event Action OnLobbyRequested;

    private void Start()
    {
        GameStateManager.OnDeathUIReady += ShowDeathPanel;
        GameStateManager.OnGameStateChanged += HandleStateChanged;

        _restartButton.onClick.AddListener(OnRestartClicked);
        _lobbyButton.onClick.AddListener(OnLobbyClicked);

        _deathPanel.SetActive(false);
        _backgroundFade.SetToHidden();
        _delayedPanelAnimation.SetToHidden();
        _ropeAnimation.SetToHidden();
    }

    private void OnDestroy()
    {
        GameStateManager.OnDeathUIReady -= ShowDeathPanel;
        GameStateManager.OnGameStateChanged -= HandleStateChanged;

        _restartButton.onClick.RemoveListener(OnRestartClicked);
        _lobbyButton.onClick.RemoveListener(OnLobbyClicked);
    }

    private void ShowDeathPanel()
    {
        DeathAnimation(true);
    }

    private void HandleStateChanged(GameState oldState, GameState newState)
    {
        if (oldState == GameState.Dead && newState != GameState.Dead)
        {
            DeathAnimation(false);
        }
    }

    private void OnRestartClicked()
    {
        OnRestartRequested?.Invoke();
    }

    private void OnLobbyClicked()
    {
        OnLobbyRequested?.Invoke();
    }

    private void DeathAnimation(bool isActive)
    {
        if (isActive)
        {
            _deathPanel.SetActive(true);
            _backgroundFade.AnimateToVisible();
            _delayedPanelAnimation.AnimateToVisible();
            _ropeAnimation.AnimateToVisible(() => RopeAnimation());
        }
        else
        {
            StopRopeAnimation();
            _backgroundFade.AnimateToHidden();
            _ropeAnimation.AnimateToHidden(() => StopRopeAnimation());
            _delayedPanelAnimation.AnimateToHidden(() =>
            {
                if (!GameStateManager.Instance.IsDead)
                {
                    _deathPanel.SetActive(false);
                }
            });
        }
    }

    private void RopeAnimation()
    {
        _ropeTween?.Kill();
        _ropeTween = _rope.transform
            .DOLocalRotate(new Vector3(0, 0, _swingAngle), _swingDuration)
            .SetEase(_swingEase)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true);
    }

    private void StopRopeAnimation()
    {
        _ropeTween?.Kill();
        _ropeTween = null;
    }
}
