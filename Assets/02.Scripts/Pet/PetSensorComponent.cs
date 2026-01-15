using UnityEngine;

public class PetSensorComponent : MonoBehaviour
{
    [SerializeField] private float _detectRange = 5f;

    private Transform _currentTarget;
    private int _monsterLayer;

    public Transform CurrentTarget => _currentTarget;
    public bool HasTarget => _currentTarget != null;

    private void Awake()
    {
        _monsterLayer = LayerMask.GetMask("Monster");
    }

    public void DetectTarget()
    {
        _currentTarget = null;

        Collider[] colliders = Physics.OverlapSphere(transform.position, _detectRange, _monsterLayer);

        float closestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<IDamageable>(out _))
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    _currentTarget = collider.transform;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectRange);
    }
}
