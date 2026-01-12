using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Key Bindings")]
    [SerializeField] private KeyCode _dashKey = KeyCode.Space;
    [SerializeField] private KeyCode _skillKey = KeyCode.Q;
    [SerializeField] private KeyCode _interactKey = KeyCode.E;

    private PlayerStateManager _stateManager;

    public event Action OnAttackInput;
    public event Action OnThrowInput;
    public event Action OnDashInput;
    public event Action OnSkillInput;
    public event Action OnInteractInput;

    public Vector2 MoveInput { get; private set; }

    private void Awake()
    {
        _stateManager = GetComponent<PlayerStateManager>();
    }
    private void Start()
    {
        _stateManager.OnPlayStateChanged += HandleCanMove;
    }
    private void OnDestroy()
    {
        _stateManager.OnPlayStateChanged -= HandleCanMove;
    }
    private void HandleCanMove(bool canInput)
    {
        this.enabled = canInput;
    }

    private void Update()
    {
        if (_stateManager.IsDead) return;

        HandleMoveInput();
        HandleActionInput();
    }

    private void HandleMoveInput()
    {
        MoveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );
    }

    private void HandleActionInput()
    {
        // 마우스 좌클릭 - 상태에 따라 다른 이벤트
        if (Input.GetMouseButtonDown(0))
        {
            if (_stateManager.IsHolding)
                OnThrowInput?.Invoke();
            else
                OnAttackInput?.Invoke();
        }

        // 마우스 홀드 - 연속 공격용
        else if (Input.GetMouseButton(0) && !_stateManager.IsHolding)
        {
            OnAttackInput?.Invoke();
        }

        if (Input.GetKeyDown(_dashKey))
            OnDashInput?.Invoke();

        if (Input.GetKeyDown(_skillKey))
            OnSkillInput?.Invoke();

        if (Input.GetKeyDown(_interactKey))
            OnInteractInput?.Invoke();
    }
}
