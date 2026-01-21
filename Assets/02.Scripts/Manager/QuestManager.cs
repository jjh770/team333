using System;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public event Action<string> OnQuestStarted;
    public event Action OnQuestCompleted;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartQuest(string questText)
    {
        Debug.Log("StartQuest");
        OnQuestStarted?.Invoke(questText);
    }

    public void CompleteQuest()
    {
        OnQuestCompleted?.Invoke();
    }
}
