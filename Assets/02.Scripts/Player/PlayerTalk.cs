using UnityEngine;

public class PlayerTalk : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private GameObject _currentTalkObjectDebug;

    private PlayerStateManager _stateManager;
    private PlayerInteraction _playerInteraction;
    private ITalkable _currentTalkable;

    public bool IsTalking => _currentTalkable != null;
    public bool CanStartTalk => !IsTalking;

    private void Awake()
    {
        _stateManager = GetComponent<PlayerStateManager>();
        _playerInteraction = GetComponent<PlayerInteraction>();
    }

    private void OnEnable()
    {
        _playerInteraction.OnInteract += HandleInteract;
    }

    private void OnDisable()
    {
        _playerInteraction.OnInteract -= HandleInteract;
    }

    private bool CanDoTalk()
    {
        return _stateManager.IsInStates(PlayerState.Idle, PlayerState.Moving, PlayerState.PickUp);
    }

    private void HandleInteract(IInteractable interactable)
    {
        // ITalkable인지 체크
        if (interactable is ITalkable talkable)
        {
            // 대화 가능한 상태인지 체크
            if (CanStartTalk && CanDoTalk())
            {
                StartTalk(talkable);
            }
        }
    }

    private void StartTalk(ITalkable talkable)
    {
        if (talkable == null) return;

        _currentTalkable = talkable;

        // 대화 시작
        talkable.Interact(gameObject);
    }
}
