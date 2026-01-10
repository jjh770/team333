using UnityEngine;

public class BullyRabbyEncounter : MonoBehaviour
{
    [Header("Monsters")]
    [SerializeField] private GameObject[] _bullyMonsters;
    [SerializeField] private GameObject _rabbyMonster;

    private BadMonsterController[] _bullyControllers;
    private int _aliveCount;

    private ItemBase _rabbyItemBase;

    [SerializeField] private UI_MonsterSpeechBubble _speechBubble;

    private void Awake()
    {
        if (_rabbyMonster != null)
        {
            _rabbyItemBase = _rabbyMonster.GetComponent<ItemBase>();
        }
    }

    private void OnEnable()
    {
        BindBullyMonsters();
        LockRabby();
    }

    private void OnDisable()
    {
        UnbindBullyMonsters();
    }

    private void BindBullyMonsters()
    {
        if (_bullyMonsters == null || _bullyMonsters.Length == 0)
        {
            _bullyControllers = System.Array.Empty<BadMonsterController>();
            _aliveCount = 0;
            return;
        }

        _bullyControllers = new BadMonsterController[_bullyMonsters.Length];
        _aliveCount = 0;

        // ÀüÃ¼ bully 
        for (int i = 0; i < _bullyMonsters.Length; i++)
        {
            var go = _bullyMonsters[i];
            if (go == null) continue;

            var bullyController = go.GetComponent<BadMonsterController>();
            if (bullyController == null) continue;

            _bullyControllers[i] = bullyController;

            if (!bullyController.IsDead)
                _aliveCount++;

            bullyController.OnDie += HandleBullyDie;
        }
    }

    private void UnbindBullyMonsters()
    {
        if (_bullyControllers == null) return;

        foreach (var controller in _bullyControllers)
        {
            if (controller == null) continue;
            controller.OnDie -= HandleBullyDie;
        }
    }

    private void HandleBullyDie(BadMonsterController deadOne)
    {
        deadOne.OnDie -= HandleBullyDie;

        _aliveCount--;
        if (_aliveCount <= 0)
        {
            UnlockRabby();
        }
    }

    private void LockRabby()
    {
        _speechBubble.SetSad();
        // _rabbyItemBase.SetLocked(true);
    }

    private void UnlockRabby()
    {
        _speechBubble.SetHappy();
        // _rabbyItemBase.SetLocked(false);
    }
}
