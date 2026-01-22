using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStep_Attack : TutorialStepBase
{
    [Header("Attack Settings")]
    [SerializeField] private int _requiredAttackCount = 3;
    [SerializeField] private float _attackCooldown = 0.3f;

    [Header("Monster Spawn")]
    [SerializeField] private GameObject _monsterPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private int _spawnCount = 1;
    
    [Header("References")]
    [SerializeField] private PlayerInputHandler _playerInputHandler;
    
    private int _currentAttackCount;
    private float _lastAttackTime;
    private readonly List<GameObject> _spawnedMonsters = new List<GameObject>();

    public int CurrentAttackCount => _currentAttackCount;
    public int RequiredAttackCount => _requiredAttackCount;

    public event Action OnAttackCountChanged;

    protected override void OnEnter()
    {
        _currentAttackCount = 0;
        _lastAttackTime = -_attackCooldown;

        if (_playerInputHandler != null)
        {
            _playerInputHandler.OnAttackInput += HandleAttackInput;
        }

        SpawnMonsters();
    }

    protected override void OnExit()
    {
        if (_playerInputHandler != null)
        {
            _playerInputHandler.OnAttackInput -= HandleAttackInput;
        }

        DespawnMonsters();
    }

    protected override void CheckCompletion()
    {
        if (_currentAttackCount >= _requiredAttackCount)
        {
            Complete();
        }
    }

    private void HandleAttackInput()
    {
        if (Time.time - _lastAttackTime < _attackCooldown) return;

        _lastAttackTime = Time.time;
        if(_currentAttackCount < _requiredAttackCount)
            _currentAttackCount++;
        OnAttackCountChanged?.Invoke();

        if (_currentAttackCount >= _requiredAttackCount)
        {
            Complete();
        }
    }

    private void SpawnMonsters()
    {
        if (_monsterPrefab == null || PoolManager.Instance == null) return;

        Vector3 spawnPosition = _spawnPoint != null ? _spawnPoint.position : transform.position;

        for (int i = 0; i < _spawnCount; i++)
        {
            Vector3 offset = new Vector3(i * 2f, 0f, 0f);
            GameObject monster = PoolManager.Instance.Get(_monsterPrefab, spawnPosition + offset, Quaternion.identity);
            _spawnedMonsters.Add(monster);
        }
    }

    private void DespawnMonsters()
    {
        if (PoolManager.Instance == null) return;

        foreach (var monster in _spawnedMonsters)
        {
            if (monster != null && monster.activeInHierarchy)
            {
                PoolManager.Instance.Return(monster);
            }
        }

        _spawnedMonsters.Clear();
    }
}
