using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Tutorial : MonoBehaviour
{
    [Header("UI Panel")]
    [SerializeField] private Image _tutorialBGPanel;
    [SerializeField] private CanvasGroup _tutorialPanel;
    [SerializeField] private float _fadeDuration = 0.3f;

    [Header("Instruction")]
    [SerializeField] private TextMeshProUGUI _instructionText;

    [Header("Skip Button")]
    [SerializeField] private Button _skipButton;

    [Header("WASD Indicators")]
    [SerializeField] private GameObject _wasdContainer;
    [SerializeField] private TextMeshProUGUI _wKeyImage;
    [SerializeField] private TextMeshProUGUI _aKeyImage;
    [SerializeField] private TextMeshProUGUI _sKeyImage;
    [SerializeField] private TextMeshProUGUI _dKeyImage;
    [SerializeField] private Color _inactiveColor = new Color(1f, 1f, 1f, 0.3f);
    [SerializeField] private Color _activeColor = new Color(1f, 1f, 1f, 1f);

    [Header("Dash Indicator")]
    [SerializeField] private GameObject _dashContainer;
    [SerializeField] private Image _dashImage;

    [Header("Attack Counter")]
    [SerializeField] private GameObject _attackCounterContainer;
    [SerializeField] private TextMeshProUGUI _attackCounterText;

    [Header("Item Interact Indicator")]
    [SerializeField] private GameObject _interactContainer;
    [SerializeField] private Image _eKeyImage;

    [Header("Tree Monster Indicator")]
    [SerializeField] private GameObject _treeMonsterContainer;
    [SerializeField] private TextMeshProUGUI _treeMonsterCountText;

    [Header("Feed Flora Indicator")]
    [SerializeField] private GameObject _feedFloraContainer;
    [SerializeField] private TextMeshProUGUI _feedFloraCountText;

    [Header("Instruction Indicator")]
    [SerializeField] private GameObject _instructionContainer;
    [SerializeField] private Image _instructionImage;

    [Header("Progress Instruction Indicator")]
    [SerializeField] private GameObject _progressInstructionContainer;
    [SerializeField] private Image _progressInstructionImage;
    [SerializeField] private Image _arrowImage;

    private TutorialStep_Movement _movementStep;
    private TutorialStep_Attack _attackStep;
    private TutorialStep_TreeMonster _treeMonsterStep;
    private TutorialStep_FeedFlora _feedFloraStep;
    private void Start()
    {
        TutorialManager.OnStepStarted += HandleStepStarted;
        TutorialManager.OnStepCompleted += HandleStepCompleted;
        TutorialManager.OnTutorialCompleted += HandleTutorialEnded;
        TutorialManager.OnTutorialSkipped += HandleTutorialEnded;

        GameStateManager.OnGameStateChanged += HandleStateChanged;

        _skipButton.onClick.AddListener(OnSkipClicked);

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        TutorialManager.OnStepStarted -= HandleStepStarted;
        TutorialManager.OnStepCompleted -= HandleStepCompleted;
        TutorialManager.OnTutorialCompleted -= HandleTutorialEnded;
        TutorialManager.OnTutorialSkipped -= HandleTutorialEnded;

        GameStateManager.OnGameStateChanged -= HandleStateChanged;

        _skipButton.onClick.RemoveListener(OnSkipClicked);

        UnsubscribeFromSteps();
    }

    private void HandleStateChanged(GameState from, GameState to)
    {
        if (to == GameState.Paused)
        {
            _tutorialPanel.gameObject.SetActive(false);
        }
        if (to == GameState.Playing)
        {
            _tutorialPanel.gameObject.SetActive(true);
        }
    }

    private void HandleStepStarted(ITutorialStep step)
    {
        UnsubscribeFromSteps();

        _instructionText.text = step.InstructionText;

        if (step is TutorialStep_Movement movementStep)
        {
            SetupMovementStep(movementStep);
        }
        else if (step is TutorialStep_Attack attackStep)
        {
            SetupAttackStep(attackStep);
        }
        else if (step is TutorialStep_ItemInteract)
        {
            SetupItemInteractStep();
        }
        else if (step is TutorialStep_TreeMonster treeMonsterStep)
        {
            SetupTreeMonsterStep(treeMonsterStep);
        }
        else if (step is TutorialStep_FeedFlora feedFloraStep)
        {
            SetupFeedFloraStep(feedFloraStep);
        }
        else if (step is TutorialStep_Instruction instructionStep)
        {
            SetupInstructionStep(instructionStep);
        }
        else if (step is TutorialStep_Dash)
        {
            SetupDashStep();
        }
        else
        {
            HideAllIndicators();
        }

        ShowPanel();
    }

    private void HandleStepCompleted(ITutorialStep step)
    {
        UnsubscribeFromSteps();
    }

    private void HandleTutorialEnded()
    {
        UnsubscribeFromSteps();
        HidePanel();
    }

    private void SetupMovementStep(TutorialStep_Movement step)
    {
        _movementStep = step;
        _movementStep.OnKeyStateChanged += UpdateMovementIndicators;

        HideAllIndicators();
        if (_wasdContainer != null) _wasdContainer.SetActive(true);

        UpdateMovementIndicators();
    }

    private void SetupAttackStep(TutorialStep_Attack step)
    {
        _attackStep = step;
        _attackStep.OnAttackCountChanged += UpdateAttackCounter;

        HideAllIndicators();
        if (_attackCounterContainer != null) _attackCounterContainer.SetActive(true);

        UpdateAttackCounter();
    }

    private void SetupItemInteractStep()
    {
        HideAllIndicators();
        if (_interactContainer != null) _interactContainer.SetActive(true);
    }

    private void SetupTreeMonsterStep(TutorialStep_TreeMonster step)
    {
        _treeMonsterStep = step;
        _treeMonsterStep.OnCountChanged += UpdateTreeMonsterCount;

        HideAllIndicators();
        if (_treeMonsterContainer != null) _treeMonsterContainer.SetActive(true);

        UpdateTreeMonsterCount();
    }

    private void SetupFeedFloraStep(TutorialStep_FeedFlora step)
    {
        _feedFloraStep = step;
        _feedFloraStep.OnCountChanged += UpdateFeedFloraCount;

        HideAllIndicators();
        if (_feedFloraContainer != null) _feedFloraContainer.SetActive(true);

        UpdateFeedFloraCount();
    }

    private void SetupInstructionStep(TutorialStep_Instruction step)
    {
        HideAllIndicators();
        if (_instructionContainer != null) _instructionContainer.SetActive(true);
        if (step.StepId == "ProgressInstruction")
        {
            _progressInstructionContainer.SetActive(true);
        }
    }

    private void SetupDashStep()
    {
        HideAllIndicators();
        if (_dashContainer != null) _dashContainer.SetActive(true);
    }

    private void HideAllIndicators()
    {
        if (_wasdContainer != null) _wasdContainer.SetActive(false);
        if (_attackCounterContainer != null) _attackCounterContainer.SetActive(false);
        if (_interactContainer != null) _interactContainer.SetActive(false);
        if (_treeMonsterContainer != null) _treeMonsterContainer.SetActive(false);
        if (_feedFloraContainer != null) _feedFloraContainer.SetActive(false);
        if (_instructionContainer != null) _instructionContainer.SetActive(false);
        if (_progressInstructionContainer != null) _progressInstructionContainer.SetActive(false);
        if (_dashContainer != null) _dashContainer.SetActive(false);
    }

    private void UnsubscribeFromSteps()
    {
        if (_movementStep != null)
        {
            _movementStep.OnKeyStateChanged -= UpdateMovementIndicators;
            _movementStep = null;
        }

        if (_attackStep != null)
        {
            _attackStep.OnAttackCountChanged -= UpdateAttackCounter;
            _attackStep = null;
        }

        if (_treeMonsterStep != null)
        {
            _treeMonsterStep.OnCountChanged -= UpdateTreeMonsterCount;
            _treeMonsterStep = null;
        }

        if (_feedFloraStep != null)
        {
            _feedFloraStep.OnCountChanged -= UpdateFeedFloraCount;
            _feedFloraStep = null;
        }
    }

    private void UpdateMovementIndicators()
    {
        if (_movementStep == null) return;

        if (_wKeyImage != null)
            _wKeyImage.color = _movementStep.WPressed ? _activeColor : _inactiveColor;

        if (_aKeyImage != null)
            _aKeyImage.color = _movementStep.APressed ? _activeColor : _inactiveColor;

        if (_sKeyImage != null)
            _sKeyImage.color = _movementStep.SPressed ? _activeColor : _inactiveColor;

        if (_dKeyImage != null)
            _dKeyImage.color = _movementStep.DPressed ? _activeColor : _inactiveColor;
    }

    private void UpdateAttackCounter()
    {
        if (_attackStep == null || _attackCounterText == null) return;

        _attackCounterText.text = $"{_attackStep.CurrentAttackCount} / {_attackStep.RequiredAttackCount}";
    }

    private void UpdateTreeMonsterCount()
    {
        if (_treeMonsterStep == null || _treeMonsterCountText == null) return;

        _treeMonsterCountText.text = $"{_treeMonsterStep.CurrentCount} / {_treeMonsterStep.RequiredCount}";
    }

    private void UpdateFeedFloraCount()
    {
        if (_feedFloraStep == null || _feedFloraCountText == null) return;

        _feedFloraCountText.text = $"{_feedFloraStep.CurrentCount} / {_feedFloraStep.RequiredCount}";
    }

    private void OnSkipClicked()
    {
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.SkipTutorial();
        }
    }

    private void ShowPanel()
    {
        if (_tutorialPanel == null) return;

        _tutorialPanel.alpha = 0f;
        _tutorialPanel.gameObject.SetActive(true);
        _tutorialPanel.DOKill();
        _tutorialPanel.DOFade(1f, _fadeDuration);
        _tutorialPanel.interactable = true;
        _tutorialPanel.blocksRaycasts = true;
        _tutorialBGPanel.DOFade(0.7f, 1f);
    }

    private void HidePanel()
    {
        if (_tutorialPanel == null) return;

        _tutorialPanel.DOKill();
        _tutorialPanel.DOFade(0f, _fadeDuration).OnComplete(() =>
        {
            _tutorialPanel.gameObject.SetActive(false);
            gameObject.SetActive(false);
        });
        _tutorialPanel.interactable = false;
        _tutorialPanel.blocksRaycasts = false;
        _tutorialBGPanel.DOFade(0f, 1f);

    }
}
