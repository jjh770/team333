using System;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private KeyCode _skillKey = KeyCode.Q;
    [SerializeField] private float _cooldown = 5f;

    private bool _isUnlocked = false;
    private float _lastUseTime = -999f;

    public bool IsUnlocked => _isUnlocked;
    public bool IsReady => Time.time >= _lastUseTime + _cooldown;

    public event Action OnSkillUnlocked;
    public event Action OnSkillUsed;

    private void Update()
    {
        if (Input.GetKeyDown(_skillKey) && _isUnlocked && IsReady)
        {
            UseSkill();
        }
    }

    public void UnlockSkill()
    {
        if (_isUnlocked) return;

        _isUnlocked = true;
        OnSkillUnlocked?.Invoke();
    }

    private void UseSkill()
    {
        _lastUseTime = Time.time;
        OnSkillUsed?.Invoke();
    }
}
