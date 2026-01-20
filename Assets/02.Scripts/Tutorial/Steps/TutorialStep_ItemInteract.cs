using UnityEngine;

public class TutorialStep_ItemInteract : TutorialStepBase
{
    [Header("Item Settings")]
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private Transform _spawnPoint;

    private GameObject _spawnedItem;
    private PlayerInteraction _playerInteraction;

    protected override void OnEnter()
    {
        Vector3 spawnPos = _spawnPoint != null ? _spawnPoint.position : transform.position;
        _spawnedItem = Instantiate(_itemPrefab, spawnPos, Quaternion.identity);

        _playerInteraction = FindObjectOfType<PlayerInteraction>();
        if (_playerInteraction != null)
        {
            _playerInteraction.OnInteract += HandleInteract;
        }
    }

    protected override void OnExit()
    {
        if (_playerInteraction != null)
        {
            _playerInteraction.OnInteract -= HandleInteract;
        }

        if (_spawnedItem != null)
        {
            Destroy(_spawnedItem);
        }
    }

    protected override void CheckCompletion() { }

    private void HandleInteract(IInteractable interactable)
    {
        if (_spawnedItem != null && interactable.Transform == _spawnedItem.transform)
        {
            Complete();
        }
    }
}
