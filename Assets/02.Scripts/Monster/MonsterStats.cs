using UnityEngine;

public class MonsterStats : MonoBehaviour
{
    [SerializeField] private ValueStat _moveSpeed;

    public float Speed => _moveSpeed.Value;

    private void Start()
    {
        InitializeStats();
    }

    private void InitializeStats()
    {
        _moveSpeed.Initialize();
    }
}
