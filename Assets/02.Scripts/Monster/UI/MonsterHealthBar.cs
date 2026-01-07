using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MonsterStat))]
public class MonsterHealthBar : MonoBehaviour
{
    [Header("체력 게이지")]
    [SerializeField] private Transform _healthBarTransform;
    [SerializeField] private Image _gaugeImage;

    private MonsterStat _stat;
    private Camera _camera;

    private void Awake()
    {
        _stat = GetComponent<MonsterStat>();
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        _stat.Heatlh.OnValueChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        _stat.Heatlh.OnValueChanged -= OnHealthChanged;
    }

    private void LateUpdate()
    {
        _healthBarTransform.forward = _camera.transform.forward;
    }

    private void OnHealthChanged(float current, float max)
    {
        _gaugeImage.fillAmount = current / max;
    }
}
