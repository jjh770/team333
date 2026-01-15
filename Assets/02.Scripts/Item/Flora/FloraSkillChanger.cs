using DG.Tweening;
using UnityEngine;

public class FloraSkillChanger : ItemBase, IAttractableByFlora
{
    [Header("Skill Settings")]
    [SerializeField] private FloraSkillBase _skillPrefab;

    private const string FloraTag = "Flora";

    [SerializeField] private float _heldSizeMultifiler = 0.7f;
    [SerializeField] private float _sizeChangeDuration = 0.5f;
    [SerializeField] private Ease _sizeChangeEase;

    private Tween _changeSizeTween;

    protected override void PickUp(Transform holder)
    {
        base.PickUp(holder);
        _changeSizeTween?.Kill();
        _changeSizeTween = transform.DOScale(Vector3.one * _heldSizeMultifiler, _sizeChangeDuration).SetEase(_sizeChangeEase);
    }

    protected override void Drop()
    {
        base.Drop();
        _changeSizeTween?.Kill();
        _changeSizeTween = transform.DOScale(Vector3.one, _sizeChangeDuration).SetEase(_sizeChangeEase);
    }

    protected override void OnCollisionEnter(Collision other)
    {
        base.OnCollisionEnter(other);

        if (_isHeld) return;

        if (!other.gameObject.CompareTag(FloraTag)) return;
        if (!other.gameObject.TryGetComponent<FloraInteraction>(out var interaction)) return;

        interaction.SetSkill(_skillPrefab);

        Destroy(gameObject);
    }
}