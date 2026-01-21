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

    private void Awake()
    {
        _positionAnimation.SetToHidden();
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
