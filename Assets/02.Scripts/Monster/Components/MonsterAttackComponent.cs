using UnityEngine;

[RequireComponent(typeof(MonsterStat))]
public class MonsterAttackComponent : MonoBehaviour
{
    protected IAnimationStateChanger _monsterController;

    protected GameObject _player;
    private MonsterStat _stat;

    private void Awake()
    {
        _monsterController = GetComponent<IAnimationStateChanger>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        // 범위 내에 플레이어가 있으면
        // 쿨타임 마다 공격
    }
}
