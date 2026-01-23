using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_EndScene : MonoBehaviour
{
    [Header("Result Display")]
    [SerializeField] private TMP_Text _clearTimeText;
    [SerializeField] private TMP_Text _rankText;

    [Header("ResultPanel")]
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private GameObject _resultNameGroup;
    [SerializeField] private TMP_Text _resultNameText;
    [SerializeField] private ParticleSystem _rankParticle;
    [SerializeField] private ParticleSystem _rankShowParticle;

    [Header("Name Input")]
    [SerializeField] private TMP_InputField _nameInputField;
    [SerializeField] private Button _submitButton;
    [SerializeField] private GameObject _inputPanel;

    [Header("Leaderboard")]
    [SerializeField] private GameObject _leaderboardPanel;
    [SerializeField] private Transform _leaderboardContainer;
    [SerializeField] private GameObject _entryPrefab;

    [Header("Buttons")]
    [SerializeField] private GameObject _buttonPanel;
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _mainMenuButton;

    [Header("Settings")]
    [SerializeField] private string _gameSceneName = "yj";
    [SerializeField] private string _mainMenuSceneName = "MainMenu";
    [SerializeField] private int _maxDisplayEntries = 5;

    [Header("Animation")]
    [SerializeField] private UI_EndSceneAnimator _animator;

    [Header("Sound")]
    [SerializeField] private UI_EndSceneSound _sound;

    [Header("Counting Animation")]
    [SerializeField] private float _timeCountDelay = 0.3f;
    [SerializeField] private float _timeCountDuration = 1.5f;
    [SerializeField] private float _rankCountDelay = 0.5f;
    [SerializeField] private float _rankCountDuration = 1f;

    [Header("Rank Animation Settings")]
    [SerializeField] private Vector3 _rankDisplayPosition = new Vector3(220, 0, -700);
    [SerializeField] private float _rankDisplayMoveDuration = 2f;
    [SerializeField] private float _layoutWaitTime = 0.1f;
    [SerializeField] private Vector3 _outOfRankPosition = new Vector3(0, -260, 0);
    [SerializeField] private float _leaderboardMoveDuration = 0.5f;

    [Header("Vignette")]
    [SerializeField] private VignetteEffector _vignetteEffector;
    [SerializeField] private float _vignetteIntensity = 1f;
    [SerializeField] private float _vignetteDuration = 2f;

    private float _clearTime;
    private bool _hasSubmitted;
    private int _rank;
    private string _currentNameText;
    private RectTransform _targetEntryPosition;

    private void Awake()
    {
        InitializeParticle();
    }

    private void OnDestroy()
    {
        DOTween.Kill(_resultPanel.transform);
        DOTween.Kill(_clearTimeText);
        DOTween.Kill(_rankText);
    }

    private void Start()
    {
        _clearTime = GameTimeManager.LastElapsedTime;
        _resultNameGroup.SetActive(false);

        SetupUI();
        SetupButtons();
        UpdateLeaderboardDisplay();
    }

    private void SetupUI()
    {
        if (_clearTimeText != null)
        {
            NumberCountingAnimator.CountToTimeWithDelay(
                _clearTimeText,
                _clearTime,
                _timeCountDelay,
                _timeCountDuration);
        }

        if (_rankText != null)
        {
            int rank = LeaderboardManager.Instance != null
                ? LeaderboardManager.Instance.GetRank(_clearTime)
                : 1;
            _rank = rank;
        }
    }

    private void SetupButtons()
    {
        if (_submitButton != null)
        {
            _submitButton.onClick.AddListener(OnSubmitClicked);
            AddHoverSound(_submitButton.gameObject);
        }

        if (_retryButton != null)
        {
            _retryButton.onClick.AddListener(OnRetryClicked);
            AddHoverSound(_retryButton.gameObject);
        }

        if (_mainMenuButton != null)
        {
            _mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            AddHoverSound(_mainMenuButton.gameObject);
        }
    }

    private void AddHoverSound(GameObject buttonObject)
    {
        EventTrigger trigger = buttonObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = buttonObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener(_ => _sound?.PlayButtonHover());
        trigger.triggers.Add(entry);
    }

    private void OnSubmitClicked()
    {
        if (_hasSubmitted) return;

        _sound?.PlayStamp();

        string playerName = _nameInputField != null ? _nameInputField.text.Trim() : "";

        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "Player";
        }

        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.AddEntry(playerName, _clearTime);
            _currentNameText = playerName;
        }

        _hasSubmitted = true;

        ShowRankAnimation();

        if (_animator != null)
        {
            _animator.HidePanel(UI_EndSceneAnimator.InputPanelName);
        }
    }

    private void UpdateLeaderboardDisplay(bool reservePlayerSlot = false)
    {
        if (_leaderboardContainer == null || _entryPrefab == null) return;
        if (LeaderboardManager.Instance == null) return;

        foreach (Transform child in _leaderboardContainer)
        {
            Destroy(child.gameObject);
        }

        var entries = LeaderboardManager.Instance.Entries;
        int displayCount = Mathf.Min(entries.Count, _maxDisplayEntries);

        for (int i = 0; i < displayCount; i++)
        {
            int displayRank = i + 1;

            // 현재 플레이어 등수 자리는 비워두고 위치만 저장
            if (reservePlayerSlot && displayRank == _rank)
            {
                var placeholder = Instantiate(_entryPrefab, _leaderboardContainer);
                _targetEntryPosition = placeholder.GetComponent<RectTransform>();

                // placeholder는 투명하게 처리 (자리만 차지)
                var canvasGroup = placeholder.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                    canvasGroup = placeholder.AddComponent<CanvasGroup>();
                canvasGroup.alpha = 0f;

                continue;
            }

            var entry = entries[i];
            var entryObject = Instantiate(_entryPrefab, _leaderboardContainer);

            var entryUI = entryObject.GetComponent<UI_LeaderboardEntry>();
            if (entryUI != null)
            {
                entryUI.Setup(displayRank, entry.Name, entry.Time);
            }
        }
    }

    private void ShowRankAnimation()
    {
        Sequence rankSequence = DOTween.Sequence();

        rankSequence
            .Append(_resultPanel.transform.DOLocalMove(_rankDisplayPosition, _rankDisplayMoveDuration));

        if (_vignetteEffector != null && _vignetteEffector.IsAvailable)
        {
            rankSequence.Join(_vignetteEffector.FadeTo(_vignetteIntensity, _vignetteDuration));
        }

        rankSequence
            .AppendCallback(() =>
            {
                NumberTextAnimation();
                _vignetteEffector?.StartPulse();
            })
            .AppendInterval(_rankCountDelay + _rankCountDuration)
            .AppendCallback(() => _vignetteEffector?.StopPulse());

        if (_vignetteEffector != null && _vignetteEffector.IsAvailable)
        {
            rankSequence.Append(_vignetteEffector.FadeTo(0f, _vignetteDuration));
        }

        rankSequence
            .AppendCallback(() =>
            {
                // 리더보드 표시 (플레이어 자리 예약)
                if (_rank == 1)
                {
                    FirstRanking(false);
                }
                UpdateLeaderboardDisplay(true);
                _animator.PlayPanel(UI_EndSceneAnimator.LeaderboardPanelName);
                _animator.PlayPanel(UI_EndSceneAnimator.ButtonPanelName);
            })
            .AppendInterval(_layoutWaitTime)
            .AppendCallback(() =>
            {
                ShowNamePanel();
                MoveResultPanelToLeaderboard();
            });
    }

    private void ShowNamePanel()
    {
        _resultNameGroup.SetActive(true);
        _resultNameText.text = _currentNameText;
    }

    private void MoveResultPanelToLeaderboard()
    {
        RectTransform resultRect = _resultPanel.GetComponent<RectTransform>();

        // 5위 밖이면 _outOfRankPosition 위치로 이동
        if (_targetEntryPosition == null)
        {
            resultRect.DOLocalMove(_outOfRankPosition, _leaderboardMoveDuration).SetEase(Ease.OutQuad);
            return;
        }

        // placeholder의 월드 위치를 resultPanel의 부모 로컬 좌표로 변환
        Vector3 targetWorldPos = _targetEntryPosition.position;
        Transform resultParent = resultRect.parent;

        Vector3 targetLocalPos;
        if (resultParent != null)
        {
            targetLocalPos = resultParent.InverseTransformPoint(targetWorldPos);
        }
        else
        {
            targetLocalPos = targetWorldPos;
        }

        // resultPanel을 해당 위치로 이동
        resultRect.DOLocalMove(targetLocalPos, _leaderboardMoveDuration).SetEase(Ease.OutQuad);
    }

    private void NumberTextAnimation()
    {
        _sound?.PlayRankCounting();

        NumberCountingAnimator.CountToWithDelay(
            _rankText,
            _rank,
            _rankCountDelay,
            _rankCountDuration,
            onComplete: () =>
            {
                if (_rank == 1)
                {
                    FirstRanking(true);
                }
            });
    }

    private void OnRetryClicked()
    {
        SceneManager.LoadScene(_gameSceneName);
    }

    private void OnMainMenuClicked()
    {
        SceneManager.LoadScene(_mainMenuSceneName);
    }

    private void InitializeParticle()
    {
        _rankParticle.Stop();
        _rankShowParticle.Stop();
    }

    private void FirstRanking(bool isShow)
    {
        if (isShow)
        {
            _rankParticle.Stop();
            _rankShowParticle.Play();
            _sound?.PlayFirstRankParticle();
        }
        else
        {
            _rankParticle.Play();
            _rankShowParticle.Stop();
        }
    }
}
