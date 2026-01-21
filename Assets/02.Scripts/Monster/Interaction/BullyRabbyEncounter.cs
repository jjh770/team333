using UnityEngine;

public class BullyRabbyEncounter : MonoBehaviour
{
    [Header("Monsters")]
    [SerializeField] private GameObject _bullyMonsterPrefab;
    [SerializeField] private Transform[] _bullyMonsterPoints;

    [Header("Rabby")]
    [SerializeField] private GameObject _rabbyMonster;
    private ItemBase _rabbyItemBase;

    private BadMonsterController[] _bullyControllers;
    private int _aliveCount;

    [SerializeField] private UI_MonsterSpeechBubble _speechBubble;

    private void Awake()
    {
        if (_rabbyMonster != null)
        {
            _rabbyItemBase = _rabbyMonster.GetComponent<ItemBase>();
        }
    }

    private void Start()
    {
        SpawnBullyMonsters();
        LockRabby();
    }

    private void OnDisable()
    {
        UnbindBullyMonsters();
    }

    private void SpawnBullyMonsters()
    {
        if (_bullyMonsterPrefab == null || _bullyMonsterPoints == null || _bullyMonsterPoints.Length == 0)
        {
            _bullyControllers = System.Array.Empty<BadMonsterController>();
            _aliveCount = 0;
            return;
        }

        _bullyControllers = new BadMonsterController[_bullyMonsterPoints.Length];
        _aliveCount = 0;

        for (int i = 0; i < _bullyMonsterPoints.Length; i++)
        {
            var point = _bullyMonsterPoints[i];
            if (point == null) continue;

            var instance = PoolManager.Instance.Get(_bullyMonsterPrefab, point.position, point.rotation);
            var controller = instance.GetComponent<BadMonsterController>();

            if (controller == null) continue;

            _bullyControllers[i] = controller;
            _aliveCount++;
            controller.OnDie += HandleBullyDie;
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
        PoolManager.Instance.Return(deadOne.gameObject);

        _aliveCount--;
        if (_aliveCount <= 0)
        {
            UnlockRabby();
        }
    }

    private void LockRabby()
    {
        _speechBubble.SetSad();
        _rabbyItemBase.SetLocked(true);
    }

    private void UnlockRabby()
    {
        _speechBubble.SetHappy();
        _rabbyItemBase.SetLocked(false);
    }

    private void Update()
    {
        if (_rabbyItemBase != null && _rabbyItemBase.IsHeld)
        {
            HideSpeechBubble();
        }
    }

    private void HideSpeechBubble()
    {
        _speechBubble.Hide();
        enabled = false;
    }
}
