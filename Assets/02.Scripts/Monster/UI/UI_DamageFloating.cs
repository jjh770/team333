using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

public class UI_DamageFloating : MonoBehaviour
{
    [SerializeField] private MonsterStat _stat;
    [SerializeField] private Vector3 _offset = new Vector3(0, 2f, 0);

    private void OnEnable()
    {
        _stat.OnDamaged += OnDamaged;
    }

    private void OnDisable()
    {
        _stat.OnDamaged -= OnDamaged;
    }

    private void OnDamaged(int damage)
    {
        Vector3 spawnPosition = transform.position + _offset;

        MMFloatingTextSpawnEvent.Trigger(
            new MMChannelData(MMChannelModes.Int, 0, null),
            spawnPosition,
            damage.ToString(),
            Vector3.up,
            1f
        );
    }
}