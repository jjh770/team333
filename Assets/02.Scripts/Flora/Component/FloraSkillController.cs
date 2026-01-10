using System;
using UnityEngine;

public class FloraSkillController : MonoBehaviour
{
    [Header("Skill Settings")]
    [SerializeField] private Transform _skillHolder;
    
    [Header("Effect")]
    [SerializeField] private FloraEffectPool _effectPool;
    
    private FloraSkillBase _currentSkill;

    public FloraSkillBase CurrentSkill => _currentSkill;
    public bool HasSkill => _currentSkill != null;

    private FloraAnimationController _animationController;
    
    public event Action<FloraSkillBase> OnSkillChanged;

    private void Awake()
    {
        if (_skillHolder == null)
        {
            _skillHolder = transform;
        }
        
        _animationController = GetComponentInChildren<FloraAnimationController>();
    }

    public void SetSkill(FloraSkillBase skillPrefab)
    {
        ClearSkill();

        if (skillPrefab == null) return;

        _currentSkill = Instantiate(skillPrefab, _skillHolder);
        _currentSkill.ResetLocalPosition();
        _currentSkill.Initialize(_effectPool);

        OnSkillChanged?.Invoke(_currentSkill);
        
        _animationController.PlaySkill();
    }

    public void ClearSkill()
    {
        if (_currentSkill == null) return;
        
        _currentSkill.DestroySkill();
        _currentSkill = null;

        OnSkillChanged?.Invoke(null);
    }
}