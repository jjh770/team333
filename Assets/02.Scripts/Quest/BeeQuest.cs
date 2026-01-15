using UnityEngine;

public class BeeQuest : MonoBehaviour
{
    //[SerializeField] private BeeSpawner _beeMonsterSpawner;
    [SerializeField] private FloraInteraction _floraInteraction;
    private QuestTree _questTree;

    public bool IsQuestCompleted { get; private set; }

    private void Awake()
    {
        _questTree = GetComponent<QuestTree>();
    }

    private void OnEnable()
    {
        if (_questTree != null)
            _questTree.OnTreeDestroyed += CompleteQuest;
    }

    private void OnDisable()
    {
        if (_questTree != null)
            _questTree.OnTreeDestroyed -= CompleteQuest;
    }

    public void CompleteQuest()
    {
        IsQuestCompleted = true;
        Debug.Log($"미션 완료!");

        //_beeMonsterSpawner.StopSpawning();
        _floraInteraction.SetMoveLock(false);
    }
}
