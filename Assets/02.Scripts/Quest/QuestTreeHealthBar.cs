using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class QuestTreeHealthBar : MonoBehaviour
{
    [Header("체력 바")]
    [SerializeField] private GameObject _healthBar;
    [SerializeField] private Transform _healthBarTransform;
    [SerializeField] private Image _gaugeImage;
    [SerializeField] private float _duration = 0.3f;
    [SerializeField] private Ease _ease = Ease.OutQuad;

    [Header("Position")]
    [SerializeField] private Vector3 _offset = new Vector3(0f, 1.5f, 0f);
    private Transform _treeTransform;

    private QuestTree _tree;
    private Transform _cameraTransform;
    private Tweener _tween;

    private void Awake()
    {
        _tree = GetComponent<QuestTree>();
        _treeTransform = transform;
        _cameraTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        _tree.OnHealthChanged += OnHealthChanged;
        _gaugeImage.fillAmount = 1f;
        _healthBar.SetActive(false);
    }

    private void OnDisable()
    {
        _tree.OnHealthChanged -= OnHealthChanged;
        _tween?.Kill();
    }

    private void LateUpdate()
    {
        _healthBarTransform.position = _treeTransform.position + _offset;
        _healthBarTransform.rotation = _cameraTransform.rotation;
    }

    private void OnHealthChanged(float current, float max)
    {
        if (!_healthBar.activeSelf)
        {
            Show();
        }

        float targetFill = current / max;

        _tween?.Kill();
        _tween = _gaugeImage.DOFillAmount(targetFill, _duration).SetEase(_ease);
    }

    public void Show()
    {
        _healthBar.SetActive(true);
    }
}