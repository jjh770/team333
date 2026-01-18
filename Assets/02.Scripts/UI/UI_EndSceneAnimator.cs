using System;
using DG.Tweening;
using UnityEngine;

public class UI_EndSceneAnimator : MonoBehaviour
{
    [Serializable]
    public class PanelAnimation
    {
        public string name;
        public RectTransform target;
        public bool playOnStart = true;
        [Header("Animation Settings")]
        public AnimationType type = AnimationType.DropFade;
        public float duration = 0.5f;
        public float delay = 0f;
        public Ease ease = Ease.OutQuad;
        [Header("Drop Settings")]
        public float startYOffset = 300f;
        public float startScale = 0.3f;
        public float fadeDuration = 0.25f;
        [Header("Slide Settings")]
        public Vector2 slideDirection = Vector2.up;

        [HideInInspector] public Vector2 originalPosition;
        [HideInInspector] public CanvasGroup canvasGroup;
        [HideInInspector] public Tween currentTween;
    }

    public enum AnimationType
    {
        DropFade,       // Y 이동 + 스케일 + 페이드
        SlideIn,        // 방향에서 슬라이드
        ScalePop,       // 스케일만
        FadeOnly        // 페이드만
    }

    [Header("Panels")]
    [SerializeField] private PanelAnimation[] _panels;

    [Header("Global Settings")]
    [SerializeField] private bool _playOnStart = true;
    [SerializeField] private float _globalDelay = 0f;

    private Sequence _masterSequence;

    private void Awake()
    {
        InitializePanels();
    }

    private void Start()
    {
        if (_playOnStart)
        {
            PlayAutoStartPanels();
        }
    }

    private void PlayAutoStartPanels()
    {
        KillAll();
        _masterSequence = DOTween.Sequence();
        _masterSequence.AppendInterval(_globalDelay);

        foreach (var panel in _panels)
        {
            if (panel.target == null) continue;

            if (panel.playOnStart)
            {
                PlayPanel(panel);
            }
            else
            {
                panel.target.gameObject.SetActive(false);
            }
        }
    }

    private void OnDestroy()
    {
        KillAll();
    }

    private void InitializePanels()
    {
        foreach (var panel in _panels)
        {
            if (panel.target == null) continue;

            panel.originalPosition = panel.target.anchoredPosition;
            panel.canvasGroup = panel.target.GetComponent<CanvasGroup>();
            if (panel.canvasGroup == null)
            {
                panel.canvasGroup = panel.target.gameObject.AddComponent<CanvasGroup>();
            }
        }
    }

    public void PlayAll()
    {
        KillAll();
        _masterSequence = DOTween.Sequence();
        _masterSequence.AppendInterval(_globalDelay);

        foreach (var panel in _panels)
        {
            if (panel.target == null) continue;
            PlayPanel(panel);
        }
    }

    public void PlayPanel(string panelName)
    {
        var panel = FindPanel(panelName);
        if (panel != null)
        {
            PlayPanel(panel);
        }
    }

    public void PlayPanel(int index)
    {
        if (index >= 0 && index < _panels.Length)
        {
            PlayPanel(_panels[index]);
        }
    }

    private void PlayPanel(PanelAnimation panel)
    {
        panel.currentTween?.Kill();
        panel.target.gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(panel.delay);

        switch (panel.type)
        {
            case AnimationType.DropFade:
                SetupDropFade(panel, seq);
                break;
            case AnimationType.SlideIn:
                SetupSlideIn(panel, seq);
                break;
            case AnimationType.ScalePop:
                SetupScalePop(panel, seq);
                break;
            case AnimationType.FadeOnly:
                SetupFadeOnly(panel, seq);
                break;
        }

        panel.currentTween = seq;
    }

    private void SetupDropFade(PanelAnimation panel, Sequence seq)
    {
        panel.target.anchoredPosition = new Vector2(panel.originalPosition.x, panel.originalPosition.y + panel.startYOffset);
        panel.target.localScale = Vector3.one * panel.startScale;
        panel.canvasGroup.alpha = 0f;

        seq.Append(panel.target.DOAnchorPosY(panel.originalPosition.y, panel.duration).SetEase(panel.ease));
        seq.Join(panel.target.DOScale(Vector3.one, panel.duration).SetEase(Ease.OutBack));
        seq.Join(panel.canvasGroup.DOFade(1f, panel.fadeDuration));
    }

    private void SetupSlideIn(PanelAnimation panel, Sequence seq)
    {
        Vector2 startPos = panel.originalPosition + panel.slideDirection * panel.startYOffset;
        panel.target.anchoredPosition = startPos;
        panel.canvasGroup.alpha = 0f;

        seq.Append(panel.target.DOAnchorPos(panel.originalPosition, panel.duration).SetEase(panel.ease));
        seq.Join(panel.canvasGroup.DOFade(1f, panel.fadeDuration));
    }

    private void SetupScalePop(PanelAnimation panel, Sequence seq)
    {
        panel.target.localScale = Vector3.zero;
        panel.canvasGroup.alpha = 1f;

        seq.Append(panel.target.DOScale(Vector3.one, panel.duration).SetEase(Ease.OutBack));
    }

    private void SetupFadeOnly(PanelAnimation panel, Sequence seq)
    {
        panel.canvasGroup.alpha = 0f;
        panel.target.localScale = Vector3.one;

        seq.Append(panel.canvasGroup.DOFade(1f, panel.duration).SetEase(panel.ease));
    }

    public void HidePanel(string panelName, float duration = 0.3f)
    {
        var panel = FindPanel(panelName);
        if (panel != null)
        {
            HidePanel(panel, duration);
        }
    }

    public void HidePanel(int index, float duration = 0.3f)
    {
        if (index >= 0 && index < _panels.Length)
        {
            HidePanel(_panels[index], duration);
        }
    }

    private void HidePanel(PanelAnimation panel, float duration)
    {
        panel.currentTween?.Kill();

        Sequence seq = DOTween.Sequence();
        seq.Append(panel.canvasGroup.DOFade(0f, duration));
        seq.Join(panel.target.DOScale(Vector3.one * 0.8f, duration).SetEase(Ease.InBack));
        seq.OnComplete(() => panel.target.gameObject.SetActive(false));

        panel.currentTween = seq;
    }

    public void ResetPanel(string panelName)
    {
        var panel = FindPanel(panelName);
        if (panel != null)
        {
            ResetPanel(panel);
        }
    }

    private void ResetPanel(PanelAnimation panel)
    {
        panel.currentTween?.Kill();
        panel.target.anchoredPosition = panel.originalPosition;
        panel.target.localScale = Vector3.one;
        panel.canvasGroup.alpha = 1f;
    }

    public void ResetAll()
    {
        KillAll();
        foreach (var panel in _panels)
        {
            if (panel.target == null) continue;
            ResetPanel(panel);
        }
    }

    private void KillAll()
    {
        _masterSequence?.Kill();
        foreach (var panel in _panels)
        {
            panel.currentTween?.Kill();
        }
    }

    private PanelAnimation FindPanel(string panelName)
    {
        foreach (var panel in _panels)
        {
            if (panel.name == panelName) return panel;
        }
        Debug.LogWarning($"Panel '{panelName}' not found.");
        return null;
    }

#if UNITY_EDITOR
    [ContextMenu("Play All Animations")]
    private void TestPlayAll()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Play 모드에서만 테스트 가능합니다.");
            return;
        }
        InitializePanels();
        PlayAll();
    }

    [ContextMenu("Reset All Animations")]
    private void TestResetAll()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Play 모드에서만 테스트 가능합니다.");
            return;
        }
        ResetAll();
    }

    [ContextMenu("Play Panel 0")]
    private void TestPlayPanel0() => TestPlayPanelAt(0);

    [ContextMenu("Play Panel 1")]
    private void TestPlayPanel1() => TestPlayPanelAt(1);

    [ContextMenu("Play Panel 2")]
    private void TestPlayPanel2() => TestPlayPanelAt(2);

    private void TestPlayPanelAt(int index)
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Play 모드에서만 테스트 가능합니다.");
            return;
        }
        if (index < _panels.Length)
        {
            InitializePanels();
            ResetPanel(_panels[index]);
            PlayPanel(index);
        }
    }
#endif
}
