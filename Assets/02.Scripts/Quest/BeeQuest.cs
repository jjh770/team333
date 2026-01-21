using UnityEngine;

public class BeeQuest : MonoBehaviour
{
    [SerializeField] private FloraInteraction _floraInteraction;
    [SerializeField] private FloraSound _floraSound;

    [Header("Quest UI")]
    [SerializeField] private SplineWaypointPath _floraPath;
    [SerializeField] private int _waypointIndex;
    [SerializeField] private string _questStartText = "길을 가로 막는 장애물을 공격해 없애세요.";

    private QuestTree _questTree;

    public bool IsQuestCompleted { get; private set; }

    private void Awake()
    {
        _questTree = GetComponent<QuestTree>();
    }

    private void OnEnable()
    {
        if (_floraPath != null)
            _floraPath.OnWaitPointReached += HandleWaitPointReached;

        if (_questTree != null)
            _questTree.OnTreeDestroyed += CompleteQuest;
    }

    private void OnDisable()
    {
        if (_floraPath != null)
            _floraPath.OnWaitPointReached -= HandleWaitPointReached;

        if (_questTree != null)
            _questTree.OnTreeDestroyed -= CompleteQuest;
    }

    private void HandleWaitPointReached(int waypointIndex)
    {
        if (waypointIndex == _waypointIndex)
        {
            QuestManager.Instance?.StartQuest(_questStartText);
        }
    }

    public void CompleteQuest()
    {
        IsQuestCompleted = true;

        _floraSound?.PlayQuestComplete();
        _floraInteraction.SetMoveLock(false);
        QuestManager.Instance?.CompleteQuest();
    }
}
