using UnityEngine;
using UnityEngine.AI;

public class BeeMonsterController : BadMonsterController
{
    [Header("Swarm Settings")]
    [SerializeField] private float _separationDistance = 2.0f; // 벌 간격
    [SerializeField] private float _separationWeight = 3.0f; // 밀어내는 힘

    [Header("Swarm Rotation")]
    [SerializeField] private float _minVelocitySqrForRotation = 0.01f;
    [SerializeField] private float _rotationLerpSpeed = 10f;

    private BeeSwarmManager _manager;
    private IPetOwnership _petOwner;
    private NavMeshAgent _agent;


    public void InitSwarm(BeeSwarmManager manager, IPetOwnership petOwner)
    {
        _manager = manager;
        _petOwner = petOwner;

        if (_agent != null)
        {
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
        }
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (_move.IsMoving == false) return;
        if (_agent == false) return;
        if (_manager == null) return;

        ApplyBoidsMovement();
    }

    private void ApplyBoidsMovement()
    {
        // 원래 가려던 방향에
        Vector3 desiredDirection = (_agent.steeringTarget - transform.position).normalized;

        // 다른 벌들과 겹치면 밀어내는 방향을 더해서
        Vector3 separationVector = Vector3.zero;
        int count = 0;

        foreach (var bee in _manager.ActiveBees)
        {
            if (bee == this) continue;
            if (bee == null) continue;

            float distance = Vector3.Distance(transform.position, bee.transform.position);
            if (distance < _separationDistance)
            {
                Vector3 pushDirection = transform.position - bee.transform.position;
                separationVector += pushDirection.normalized / distance;
                count++;
            }
        }

        // 최종 이동 방향을 만들고
        Vector3 finalVelocity = desiredDirection;
        if (count > 0)
        {
            finalVelocity += separationVector * _separationWeight;
        }

        finalVelocity = finalVelocity.normalized * _stat.GetMoveSpeed();

        // 그 방향으로 내비매쉬에 속도를 직접 세팅하고
        _agent.velocity = finalVelocity;

        // 이동 방향을 바라보게 회전시킨다.
        if (finalVelocity.sqrMagnitude > _minVelocitySqrForRotation)
        {
            Quaternion targetRot = Quaternion.LookRotation(finalVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * _rotationLerpSpeed);
        }
    }

    protected override void Die()
    {
        if (_isDead) return;
        _isDead = true;

        _move.Stop();

        if (_petOwner != null && _petOwner.HasPet() == false)
        {
            _itemDrop.DropItem(_stat.Data.DropItem);
        }

        ApplyState(MonsterState.Die);
        _deathRoutine = StartCoroutine(DeathCoroutine());
    }
}