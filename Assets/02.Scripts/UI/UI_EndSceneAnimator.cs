using DG.Tweening;
using System;
using UnityEngine;

public class UI_EndSceneAnimator : MonoBehaviour
{
    [Serializable]
    public class PanelAnimation
    {
        public string Name;
        public RectTransform Target;
        public bool PlayOnStart = true;

        [Header("Animation Settings")]
        public AnimationType Type = AnimationType.DropFade;
        public float Duration = 0.5f;
        public float Delay = 0f;
        public Ease Ease = Ease.OutQuad;

        [Header("Position Settings")]
        public Vector2 StartPosition;
        public Vector2 EndPosition;

        [Header("Rotation Settings")]
        public Vector3 StartRotation;
        public Vector3 EndRotation;

        [Header("Scale Settings")]
        public float StartScale = 0.3f;

        [Header("Fade Settings")]
        public float FadeDuration = 0.25f;

        [HideInInspector] public CanvasGroup CanvasGroup;
        [HideInInspector] public Tween CurrentTween;
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

    public const string InputPanelName = "InputPanel";
    public const string LeaderboardPanelName = "LeaderboardPanel";
    public const string ButtonPanelName = "ButtonPanel";

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
            if (panel.Target == null) continue;

            if (panel.PlayOnStart)
            {
                PlayPanel(panel);
            }
            else
            {
                panel.Target.gameObject.SetActive(false);
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
            if (panel.Target == null) continue;

            panel.CanvasGroup = panel.Target.GetComponent<CanvasGroup>();
            if (panel.CanvasGroup == null)
            {
                panel.CanvasGroup = panel.Target.gameObject.AddComponent<CanvasGroup>();
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
            if (panel.Target == null) continue;
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
        panel.CurrentTween?.Kill();
        panel.Target.gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(panel.Delay);

        switch (panel.Type)
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

        panel.CurrentTween = seq;
    }

    private void SetupDropFade(PanelAnimation panel, Sequence seq)
    {
        panel.Target.anchoredPosition = panel.StartPosition;
        panel.Target.localEulerAngles = panel.StartRotation;
        panel.Target.localScale = Vector3.one * panel.StartScale;
        panel.CanvasGroup.alpha = 0f;

        seq.Append(panel.Target.DOAnchorPos(panel.EndPosition, panel.Duration).SetEase(panel.Ease));
        seq.Join(panel.Target.DOLocalRotate(panel.EndRotation, panel.Duration).SetEase(panel.Ease));
        seq.Join(panel.Target.DOScale(Vector3.one, panel.Duration).SetEase(Ease.OutBack));
        seq.Join(panel.CanvasGroup.DOFade(1f, panel.FadeDuration));
    }

    private void SetupSlideIn(PanelAnimation panel, Sequence seq)
    {
        panel.Target.anchoredPosition = panel.StartPosition;
        panel.Target.localEulerAngles = panel.StartRotation;
        panel.CanvasGroup.alpha = 0f;

        seq.Append(panel.Target.DOAnchorPos(panel.EndPosition, panel.Duration).SetEase(panel.Ease));
        seq.Join(panel.Target.DOLocalRotate(panel.EndRotation, panel.Duration).SetEase(panel.Ease));
        seq.Join(panel.CanvasGroup.DOFade(1f, panel.FadeDuration));
    }

    private void SetupScalePop(PanelAnimation panel, Sequence seq)
    {
        panel.Target.localScale = Vector3.zero;
        panel.CanvasGroup.alpha = 1f;

        seq.Append(panel.Target.DOScale(Vector3.one, panel.Duration).SetEase(Ease.OutBack));
    }

    private void SetupFadeOnly(PanelAnimation panel, Sequence seq)
    {
        panel.CanvasGroup.alpha = 0f;
        panel.Target.localScale = Vector3.one;

        seq.Append(panel.CanvasGroup.DOFade(1f, panel.Duration).SetEase(panel.Ease));
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
        panel.CurrentTween?.Kill();

        Sequence seq = DOTween.Sequence();
        seq.Append(panel.CanvasGroup.DOFade(0f, duration));
        seq.Join(panel.Target.DOScale(Vector3.one * 0.8f, duration).SetEase(Ease.InBack));
        seq.OnComplete(() => panel.Target.gameObject.SetActive(false));

        panel.CurrentTween = seq;
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
        panel.CurrentTween?.Kill();
        panel.Target.anchoredPosition = panel.EndPosition;
        panel.Target.localEulerAngles = panel.EndRotation;
        panel.Target.localScale = Vector3.one;
        panel.CanvasGroup.alpha = 1f;
    }

    public void ResetAll()
    {
        KillAll();
        foreach (var panel in _panels)
        {
            if (panel.Target == null) continue;
            ResetPanel(panel);
        }
    }

    private void KillAll()
    {
        _masterSequence?.Kill();
        foreach (var panel in _panels)
        {
            panel.CurrentTween?.Kill();
        }
    }

    private PanelAnimation FindPanel(string panelName)
    {
        foreach (var panel in _panels)
        {
            if (panel.Name == panelName) return panel;
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
#endif
}
