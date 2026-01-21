using UnityEngine;

public class PlayerWeaponVisual : MonoBehaviour
{
    [SerializeField] private GameObject _weapon;

    private PlayerStateManager _stateManager;

    private void Awake()
    {
        _stateManager = GetComponent<PlayerStateManager>();
    }

    private void OnEnable()
    {
        _stateManager.OnStateChanged += OnStateChanged;
    }

    private void OnDisable()
    {
        _stateManager.OnStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(PlayerState from, PlayerState to)
    {
        bool showWeapon = to != PlayerState.PickUp && to != PlayerState.Throw && to != PlayerState.Clear;
        _weapon.SetActive(showWeapon);
    }
}
