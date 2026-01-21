using TMPro;
using UnityEngine;

public class UI_Quest : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _questPanel;
    [SerializeField] private TextMeshProUGUI _questText;

    private void Start()
    {
        if (_questPanel != null) _questPanel.SetActive(false);

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
    }

    private void ShowQuest(string text)
    {
        Debug.Log("ShowQuest");
        if (_questText != null) _questText.text = text;
        if (_questPanel != null) _questPanel.SetActive(true);
    }

    private void HideQuest()
    {
        if (_questPanel != null) _questPanel.SetActive(false);
    }
}
