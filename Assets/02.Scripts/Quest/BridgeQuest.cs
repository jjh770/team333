using UnityEngine;

public class BridgeQuest : MonoBehaviour
{
    [SerializeField] private int _neededPlankCount = 9;
    [SerializeField] private GameObject[] _plankObjects;
    [SerializeField] private GameObject[] _plankOutlineObjects;
    [SerializeField] private PlankMonsterSpawner _plankMonsterSpawner;
    [SerializeField] private FloraInteraction _floraInteraction;
    [SerializeField] private FloraSound _floraSound;

    [Header("Quest UI")]
    [SerializeField] private SplineWaypointPath _floraPath;
    [SerializeField] private int _waypointIndex;
    [SerializeField] private string _questStartText = "판자를 다리에 던져 끊어진 \n다리를 연결하세요.";
    [SerializeField] private string _hintStartText = "여기 나무는 생긴게 다르네?";

    private int _currentPlankCount = 0;
    public bool IsQuestCompleted { get; private set; }

    private void OnEnable()
    {
        if (_floraPath != null)
        {
            _floraPath.OnWaitPointReached += HandleWaitPointReached;
        }
    }

    private void OnDisable()
    {
        if (_floraPath != null)
        {
            _floraPath.OnWaitPointReached -= HandleWaitPointReached;
        }
    }

    private void Start()
    {
        foreach (GameObject obj in _plankObjects)
        {
            obj.SetActive(false);
        }

        foreach (GameObject obj in _plankOutlineObjects)
        {
            obj.SetActive(false);
        }

        _plankOutlineObjects[_currentPlankCount].SetActive(true);
    }

    private void HandleWaitPointReached(int waypointIndex)
    {
        if (waypointIndex == _waypointIndex)
        {
            UpdateQuestUI();
        }
    }

    private void UpdateQuestUI()
    {
        string questText = $"{_questStartText}\n판자 {_currentPlankCount}/{_neededPlankCount}";
        QuestManager.Instance?.StartQuest(questText, _hintStartText);
    }

    public void AddPlank()
    {
        if (IsQuestCompleted) return;
        if (_currentPlankCount >= _plankObjects.Length || _currentPlankCount >= _plankOutlineObjects.Length) return;

        _plankObjects[_currentPlankCount].SetActive(true);
        _plankOutlineObjects[_currentPlankCount].SetActive(false);
        _floraSound?.PlayBridgeQuestSound();

        _currentPlankCount++;

        if (_currentPlankCount < _plankOutlineObjects.Length)
        {
            _plankOutlineObjects[_currentPlankCount].SetActive(true);
        }

        if (_currentPlankCount == _neededPlankCount)
        {
            CompleteQuest();
        }
        else
        {
            UpdateQuestUI();
        }
    }

    private void CompleteQuest()
    {
        IsQuestCompleted = true;

        _plankMonsterSpawner.StopSpawning();
        _floraInteraction.SetMoveLock(false);
        _floraSound?.PlayQuestComplete();
        QuestManager.Instance?.CompleteQuest();
    }
}
