using DG.Tweening;
using UnityEngine;

public class PetDirectionIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _indicator;
    [SerializeField] private Transform _pet;

    [Header("Show Settings")]
    [SerializeField] private float _showDuration = 1f;
    [SerializeField] private Ease _showEase;
    [SerializeField] private float _sizeMultiplier = 0.6f;

    private MeshRenderer _indicatorRenderer;
    private Tweener _showTween;

    private void Awake()
    {
        _indicatorRenderer = _indicator.GetComponent<MeshRenderer>();
        _indicator.transform.localScale = Vector3.zero;
    }

    private void Start()
    {
        ShowIn();
    }

    private void Update()
    {
        _indicator.transform.rotation = Quaternion.LookRotation(_pet.forward);
    }

    private void ShowIn()
    {
        _showTween?.Kill();
        _indicatorRenderer.enabled = true;
        _showTween = _indicator.transform.DOScale(Vector3.one * _sizeMultiplier, _showDuration).SetEase(_showEase);
    }
}
