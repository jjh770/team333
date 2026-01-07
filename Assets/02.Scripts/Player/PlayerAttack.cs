using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private float _attackDamage = 10f;
    [SerializeField] private float _attackCooldown = 0.5f;
    [SerializeField] private LayerMask _enemyLayer;

    [Header("Attack Point")]
    [SerializeField] private Transform _attackPoint;

    private float _attackCooldownTimer;
    private bool _canAttack = true;

    private void Update()
    {
        UpdateCooldown();
        HandleAttackInput();
    }

    private void UpdateCooldown()
    {
        if (_attackCooldownTimer > 0)
        {
            _attackCooldownTimer -= Time.deltaTime;
        }
    }

    private void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0) && _canAttack && _attackCooldownTimer <= 0)
        {
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        _attackCooldownTimer = _attackCooldown;

        Vector3 attackPosition = _attackPoint != null ? _attackPoint.position : transform.position + transform.forward;

        Collider[] hitEnemies = Physics.OverlapSphere(attackPosition, _attackRange, _enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log($"Hit: {enemy.name}");

            // TODO: 적에게 데미지를 주는 로직 추가
            // 예: enemy.GetComponent<EnemyHealth>()?.TakeDamage(_attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_attackPoint.position, _attackRange);
    }

    public bool CanAttack
    {
        get => _canAttack;
        set => _canAttack = value;
    }

    public float CooldownRemaining => _attackCooldownTimer;
}
