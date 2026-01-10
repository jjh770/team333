using UnityEngine;

public class MonsterItemDropComponent : MonoBehaviour
{
    [SerializeField] private GameObject _dropItem;
    [SerializeField] private float _yOffset = 0.5f;

    public void DropItem()
    {
        if (_dropItem == null) return;
        if (ItemFactory.Instance == null) return;

        Vector3 offset = new Vector3(0f, _yOffset, 0f);
        Vector3 spawnPos = transform.position + offset;

        ItemFactory.Instance.Spawn(_dropItem, spawnPos, Quaternion.identity);
    }
}
