using UnityEngine;

public class PlayerTalk : MonoBehaviour
{
    private PlayerStateManager _stateManager;
    private PlayerInteraction _playerInteraction;
    private IInteractable _currentTalkTarget;

    public bool IsTalking => _currentTalkTarget != null;
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

    private bool IsTalkInteraction(InteractionType type)
    {
        return type == InteractionType.TalkToMove || type == InteractionType.TalkToSpeedUp;
    }

    private void HandleInteract(IInteractable interactable)
    {
        if (interactable == null) return;

        // Talk 타입인지 체크
        if (IsTalkInteraction(interactable.Type))
        {
            // 대화 가능한 상태인지 체크
            if (CanStartTalk && CanDoTalk())
            {
                StartTalk(interactable);
            }
        }
    }

    private void StartTalk(IInteractable talkTarget)
    {
        if (talkTarget == null) return;

        _currentTalkTarget = talkTarget;

        // 대화 시작 (Interact는 PlayerInteraction에서 이미 호출됨)
    }
}
