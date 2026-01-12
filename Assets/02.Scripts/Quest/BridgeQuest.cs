using System;
using UnityEngine;

public class BridgeQuest : MonoBehaviour
{
    [SerializeField] private int _neededPlankCount = 9;
    [SerializeField] private GameObject[] _plankObjects;
    [SerializeField] private PlankMonsterSpawner _plankMonsterSpawner;
    [SerializeField] private FloraInteraction _floraInteraction;
    
    private int _currentPlankCount = 0;
    public bool IsQuestCompleted { get; private set; }

    private void Start()
    {
        foreach (GameObject obj in _plankObjects)
        {
            obj.SetActive(false);
        }
    }
    public void AddPlank()
    {
        if (IsQuestCompleted) return;
        if (_currentPlankCount >= _plankObjects.Length) return;

        _plankObjects[_currentPlankCount].SetActive(true);
        _currentPlankCount++;

        if (_currentPlankCount == _neededPlankCount)
        {
            CompleteQuest();
        }
    }

    private void CompleteQuest()
    {
        IsQuestCompleted = true;
        Debug.Log($"미션 완료!");

        _plankMonsterSpawner.StopSpawning();
        _floraInteraction.SetMoveLock(false);
    }
}
