using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerStat))]
public class PlayerHealthBar : MonoBehaviour
{
    [Header("체력 바 설정")]
    [SerializeField] private Transform _healthBarTransform;
    [SerializeField] private Image _gaugeImage;
    [SerializeField] private float _duration = 0.3f;
    [SerializeField] private Ease _ease = Ease.OutQuad;

    private PlayerStat _stat;
    private Transform _cameraTransform;
    private Tweener _tween;

    private void Awake()
    {
        _stat = GetComponent<PlayerStat>();
        _cameraTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        _stat.OnHealthChanged += OnHealthChanged;
        _gaugeImage.fillAmount = 1f;
    }

    private void OnDisable()
    {
        _stat.OnHealthChanged -= OnHealthChanged;
        _tween?.Kill();
    }

    private void LateUpdate()
    {
        _healthBarTransform.forward = _cameraTransform.forward;
    }

    private void OnHealthChanged(float current, float max)
    {
        float targetFill = current / max;

        _tween?.Kill();
        _tween = _gaugeImage.DOFillAmount(targetFill, _duration).SetEase(_ease);
    }
}
