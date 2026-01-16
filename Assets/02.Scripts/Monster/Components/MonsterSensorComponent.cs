using UnityEngine;

public enum MonsterTargetType
{
    Player,
    Flora
}

public class MonsterSensorComponent : MonoBehaviour
{
    [Header("Settings")]
    public MonsterTargetType Target = MonsterTargetType.Player;

    private Transform _currentTargetTransform;

    private void OnEnable()
    {
        FindTarget();
    }

    public void FindTarget()
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
