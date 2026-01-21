using DG.Tweening;
using UnityEngine;

public class PlayerPet : MonoBehaviour, IPetOwnership
{
    [Header("Pet Settings")]
    [SerializeField] private GameObject _petPrefab;
    [SerializeField] private MonsterSound _monsterSound;

    [Header("Spawn position")]
    [SerializeField] private float _spawnDuration = 0.5f;
    [SerializeField] private float _spawnHeight = 2f;
    [SerializeField] private Ease _spawnEase = Ease.OutBack;

    private int _currentCount = 0;

    private PetController _currentPet;

    public void TryAddPet()
    {
        if (_petPrefab == null) return;
        if (HasPet()) return;

        SpawnPet();
    }

    private void SpawnPet()
    {
        _currentCount++;
        _monsterSound?.PetAppear();
        BeezPetItem.DisableAll();

        // 플레이어 뒤쪽에 스폰
        Vector3 spawnPosition = transform.position - transform.forward * 1.5f;
        spawnPosition.y += _spawnHeight;

        GameObject petObj = Instantiate(_petPrefab, spawnPosition, Quaternion.identity);
        _currentPet = petObj.GetComponent<PetController>();

        petObj.transform.localScale = Vector3.zero;
        petObj.transform.DOScale(Vector3.one, _spawnDuration).SetEase(_spawnEase);

        Vector3 targetPosition = spawnPosition;
        targetPosition.y = transform.position.y + 0.5f;
        petObj.transform.DOMove(targetPosition, _spawnDuration)
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
                if (_currentPet != null)
                {
                    _currentPet.Initialize(transform);
                }
            });
    }

    public bool HasPet()
    {
        return _currentCount > 0;
    }
}
