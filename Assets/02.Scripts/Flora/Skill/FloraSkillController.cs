using System;
using UnityEngine;

public class FloraSkillController : MonoBehaviour
{
    [Header("Skill Settings")]
    [SerializeField] private Transform _skillHolder;

    private FloraSkillBase _currentSkill;

    public FloraSkillBase CurrentSkill => _currentSkill;
    public bool HasSkill => _currentSkill != null;

    public event Action<FloraSkillBase> OnSkillChanged;

    private void Awake()
    {
        if (_skillHolder == null)
        {
            _skillHolder = transform;
        }
    }

    public void SetSkill(FloraSkillBase skillPrefab)
    {
        ClearSkill();

        if (skillPrefab == null) return;

        _currentSkill = Instantiate(skillPrefab, _skillHolder);
        _currentSkill.transform.localPosition = Vector3.zero;


        OnSkillChanged?.Invoke(_currentSkill);
    }

    public void ClearSkill()
    {
        if (_currentSkill == null) return;

        Destroy(_currentSkill.gameObject);
        _currentSkill = null;

        OnSkillChanged?.Invoke(null);
    }
}