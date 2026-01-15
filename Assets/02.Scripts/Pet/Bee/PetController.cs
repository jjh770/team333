using UnityEngine;

[RequireComponent(typeof(PetMovementComponent))]
public class PetController : MonoBehaviour
{
    private PetMovementComponent _movement;
    private bool _isInitialized;

    private void Awake()
    {
        _movement = GetComponent<PetMovementComponent>();
    }

    public void Initialize(Transform player)
    {
        _movement.Initialize(player);
        _isInitialized = true;
    }

    private void Update()
    {
        if (!_isInitialized) return;

        _movement.FollowPlayer(Time.deltaTime);
        _movement.LookPlayer();
    }
}
