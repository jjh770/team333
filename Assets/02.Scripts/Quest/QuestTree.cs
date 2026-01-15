using UnityEngine;

public class QuestTree : MonoBehaviour, IDamageable
{
    [SerializeField] private ConsumableStat _health;
    private BeeQuest _beeQuest;

    private void Awake()
    {
        _beeQuest = GetComponent<BeeQuest>();
    }
    
    public bool TryTakeDamage(Damage damage)
    {
        if (damage.Value <= 0) return false;
        
        _health.Decrease(damage.Value);
        Debug.Log(_health.CurrentValue);

        if (_health.IsEmpty)
        {
            Destroy(gameObject);
            _beeQuest.CompleteQuest();
        }
        return true;
    }
}
