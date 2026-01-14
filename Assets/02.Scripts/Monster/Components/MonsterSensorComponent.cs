using UnityEngine;

public enum MonsterTargetType
{
    Player,
    Flora,
    Both
}

public class MonsterSensorComponent : MonoBehaviour
{
    [Header("Settings")]
    public MonsterTargetType Target = MonsterTargetType.Player;

    [Header("Target")]
    [SerializeField] private float _playerNearDistance = 7f;
    [SerializeField] private float _targetUpdateInterval = 0.3f;
    [SerializeField] private float _targetUpdateTimer = 0.2f;

    private Transform _currentTargetTransform;

    private void OnEnable()
    {
        FindTarget();
    }

    private void FindTarget()
    {
        GameObject foundObject = null;

        switch (Target)
        {
            case MonsterTargetType.Player:
                foundObject = GameObject.FindGameObjectWithTag("Player");
                break;
            case MonsterTargetType.Flora:
                foundObject = GameObject.FindGameObjectWithTag("Flora");
                break;
        }

        if (foundObject != null)
        {
            _currentTargetTransform = foundObject.transform;
        }
    }

    public Transform GetCurrentTarget()
    {
        return _currentTargetTransform;
    }
}
