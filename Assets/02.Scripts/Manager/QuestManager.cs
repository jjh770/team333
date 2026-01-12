using System;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private float _needBoardCount = 15;
    [SerializeField] private FloraInventory _floraInventory;

    public bool IsQuestCompleted { get; private set; }

    private void OnEnable()
    {
        if (_floraInventory != null)
        {
            _floraInventory.OnBoardChanged += HandleBoardChanged;
        }
    }

    private void OnDisable()
    {
        if (_floraInventory != null)
        {
            _floraInventory.OnBoardChanged -= HandleBoardChanged;
        }
    }

    private void HandleBoardChanged(float boardCount)
    {
        if (IsQuestCompleted) return;

        if (boardCount >= _needBoardCount)
        {
            CompleteQuest();
        }
    }

    private void CompleteQuest()
    {
        IsQuestCompleted = true;
        Debug.Log($"미션 완료!");
    }
}
