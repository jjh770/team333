using DG.Tweening;
using UnityEngine;

public class Wood : ItemBase, IAttractableByFlora
{
    [Header("Settings")]
    [SerializeField] private int _addWoodAmount = 1;

    [Header("Spin Settings")]
    [SerializeField] private float _spinSpeed = 360f;

    private const string FloraTag = "Flora";
    private ItemFactory _itemFactory;
    private int _monsterLayer;
    private Tween _spinTween;

    public override IconType IconType => IconType.Wood;

    override protected void Awake()
    {
        base.Awake();
        _itemFactory = ItemFactory.Instance;
        _monsterLayer = LayerMask.NameToLayer("Monster");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        StartSpin();
    }

    private void OnDisable()
    {
        StopSpin();
    }

    private void StartSpin()
    {
        StopSpin();
        _spinTween = transform.DORotate(new Vector3(0f, 360f, 0f), 360f / _spinSpeed, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    private void StopSpin()
    {
        _spinTween?.Kill();
        _spinTween = null;
    }

    protected override void OnCollisionEnter(Collision other)
    {
        base.OnCollisionEnter(other);

        if (other.gameObject.layer != _monsterLayer)
        {
            StopSpin();
        }

        if (_isHeld) return;

        if (!other.gameObject.CompareTag(FloraTag)) return;
        if (!other.gameObject.TryGetComponent<FloraInteraction>(out var interaction)) return;

        interaction.AddWood(_addWoodAmount, transform.position);
        _itemFactory.ReturnItem(this.gameObject);
    }
}
