using UnityEngine;
using UnityEngine.UI;

public class UI_LobbyLeaderboard : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject _leaderboardPanel;

    [Header("Leaderboard")]
    [SerializeField] private Transform _leaderboardContainer;
    [SerializeField] private GameObject _entryPrefab;
    [SerializeField] private int _maxDisplayEntries = 5;

    [Header("Buttons")]
    [SerializeField] private Button _closeButton;

    private void Awake()
    {
        if (_leaderboardPanel != null)
        {
            _leaderboardPanel.SetActive(false);
        }
    }

    private void Start()
    {
        if (_closeButton != null)
        {
            _closeButton.onClick.AddListener(Hide);
        }
    }

    private void OnDestroy()
    {
        if (_closeButton != null)
        {
            _closeButton.onClick.RemoveListener(Hide);
        }
    }

    public void Show()
    {
        UpdateLeaderboardDisplay();
        _leaderboardPanel.SetActive(true);
    }

    public void Hide()
    {
        _leaderboardPanel.SetActive(false);
    }

    private void UpdateLeaderboardDisplay()
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
            var entry = entries[i];
            var entryObject = Instantiate(_entryPrefab, _leaderboardContainer);

            var entryUI = entryObject.GetComponent<UI_LeaderboardEntry>();
            if (entryUI != null)
            {
                entryUI.Setup(i + 1, entry.Name, entry.Time);
            }
        }
    }
}
