using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _woodCount;
    [SerializeField] private FloraInventory _floraInventory;
    [SerializeField] private RectTransform _animationTarget;

    [Header("Animation Settings")]
    [SerializeField] private float _punchScale = 0.2f;
    [SerializeField] private float _duration = 0.3f;

    private Tweener _scaleTween;

    private void OnEnable()
    {
        _floraInventory.OnWoodChanged += UpdateUI;
    }

    private void OnDisable()
    {
        _floraInventory.OnWoodChanged -= UpdateUI;
    }

    private void UpdateUI(float value)
    {
        _woodCount.text = ((int)value).ToString();
        PlayPunchAnimation();
    }

    private void PlayPunchAnimation()
    {
        if (_animationTarget == null) return;

        _scaleTween?.Kill();
        _animationTarget.localScale = Vector3.one;
        _scaleTween = _animationTarget.DOPunchScale(Vector3.one * _punchScale, _duration, 1, 0.5f);
    }

    private void OnDestroy()
    {
        _scaleTween?.Kill();
    }
}
