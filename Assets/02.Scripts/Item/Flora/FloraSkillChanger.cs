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
    [SerializeField] private IconType _iconType;
    private Tween _changeSizeTween;

    public override IconType IconType => _iconType;

    protected override void PickUp(Transform holder)
    {
        base.PickUp(holder);
        AnimateSize(Vector3.one * _heldSizeMultifiler);
    }

    protected override void Drop()
    {
        base.Drop();
        AnimateSize(Vector3.one);
    }

    private void AnimateSize(Vector3 targetScale)
    {
        _changeSizeTween?.Kill();
        _changeSizeTween = transform.DOScale(targetScale, _sizeChangeDuration).SetEase(_sizeChangeEase);
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