using DG.Tweening;
using GameUI.Animations;
using TMPro;
using UnityEngine;

public class UI_Quest : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _questPanel;
    [SerializeField] private TextMeshProUGUI _questText;
    [SerializeField] private TextMeshProUGUI _hintText;
    [Header("위치 애니메이션")]
    [SerializeField] private UIElementAnimation _positionAnimation;

    [Header("플레이어 가림 처리")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private RectTransform _questRectTransform;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _transparentAlpha = 0.3f;
    [SerializeField] private float _fadeDuration = 0.2f;

    private Camera _mainCamera;
    private Canvas _canvas;
    private bool _isTransparent;
    private Tween _fadeTween;

    private void Awake()
    {
        _positionAnimation.SetToHidden();
        _mainCamera = Camera.main;
        _canvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestStarted += ShowQuest;
            QuestManager.Instance.OnQuestCompleted += HideQuest;
        }
    }

    private void OnDisable()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestStarted -= ShowQuest;
            QuestManager.Instance.OnQuestCompleted -= HideQuest;
        }
        _fadeTween?.Kill();
    }

    private void Update()
    {
        UpdateTransparency();
    }

    private void UpdateTransparency()
    {
        if (_playerTransform == null || _questRectTransform == null || _canvasGroup == null) return;

        bool isOverlapping = IsPlayerOverlappingUI();

        if (isOverlapping && !_isTransparent)
        {
            _isTransparent = true;
            _fadeTween?.Kill();
            _fadeTween = _canvasGroup.DOFade(_transparentAlpha, _fadeDuration);
        }
        else if (!isOverlapping && _isTransparent)
        {
            _isTransparent = false;
            _fadeTween?.Kill();
            _fadeTween = _canvasGroup.DOFade(1f, _fadeDuration);
        }
    }

    private bool IsPlayerOverlappingUI()
    {
        Vector3 screenPos = _mainCamera.WorldToScreenPoint(_playerTransform.position);

        // 플레이어가 카메라 뒤에 있으면 무시
        if (screenPos.z < 0) return false;

        return RectTransformUtility.RectangleContainsScreenPoint(_questRectTransform, screenPos, _canvas.worldCamera);
    }

    private void ShowQuest(string quest, string hint)
    {
        if (_questText != null) _questText.text = quest;
        if (_hintText != null) _hintText.text = hint;
        _positionAnimation.AnimateToVisible();
    }

    private void HideQuest()
    {
        _positionAnimation.AnimateToHidden();
    }
}
