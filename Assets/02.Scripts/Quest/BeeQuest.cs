using UnityEngine;

public class BeeQuest : MonoBehaviour
{
    //[SerializeField] private BeeSpawner _beeMonsterSpawner;
    [SerializeField] private FloraInteraction _floraInteraction;
    
    public bool IsQuestCompleted { get; private set; }

    public void CompleteQuest()
    {
        IsQuestCompleted = true;
        Debug.Log($"미션 완료!");

        //_beeMonsterSpawner.StopSpawning();
        _floraInteraction.SetMoveLock(false);
    }
}
