using GameUI.Animations;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_PlayerSkillCoolBar : MonoBehaviour
{
    [Header("플레이어 스킬 참조")]
    [SerializeField] private PlayerSkillController _skillController;

    [Header("UI 요소 - UI_SkillCoolBar")]
    [Tooltip("CurrentBar")][SerializeField] private RectTransform _coolBarCurrentBar;
    [Tooltip("SkillKeyIconText")][SerializeField] private TextMeshProUGUI _keyIconText;

    [Header("위치 애니메이션")]
    [SerializeField] private List<UIElementAnimation> _positionAnimations;

    [Header("페이드 애니메이션")]
    [SerializeField] private List<UIFadeAnimation> _fadeAnimations;

    private void Awake()
    {
        foreach (var anim in _positionAnimations)
        {
            anim.SetToHidden();
        }

        foreach (var fade in _fadeAnimations)
        {
            fade.SetToHidden();
        }
    }

    private void Start()
    {
        if (_skillController != null)
        {
            _skillController.OnSkillUnlocked += OnSkillCoolBarShow;
        }
    }

    private void OnDestroy()
    {
        if (_skillController != null)
        {
            _skillController.OnSkillUnlocked -= OnSkillCoolBarShow;
        }
    }

    private void Update()
    {
        if (_skillController == null) return;
        if (!_skillController.IsUnlocked) return;

        UpdateCoolBarScale();
        UpdateKeyIconText();
    }

    private void OnSkillCoolBarShow()
    {
        foreach (var anim in _positionAnimations)
        {
            anim.AnimateToVisible();
        }

        foreach (var fade in _fadeAnimations)
        {
            fade.AnimateToVisible();
        }
    }

    private void UpdateCoolBarScale()
    {
        Vector3 scale = _coolBarCurrentBar.localScale;
        scale.x = _skillController.CooldownProgress;
        _coolBarCurrentBar.localScale = scale;
    }

    private void UpdateKeyIconText()
    {
        if (_skillController.IsReady)
        {
            _keyIconText.color = Color.white;
        }
        else
        {
            _keyIconText.color = Color.black;
        }
    }
}
