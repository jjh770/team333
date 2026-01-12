using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpThrow : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _maxPickUpCount = 2;
    [SerializeField] private Transform[] _holdPoints;
    [SerializeField] private float _throwForce = 10f;
    [SerializeField] private float _throwUpwardAngle = 0.5f;

    [Header("Debug")]
    [SerializeField] private List<IPickable> _heldObjects = new List<IPickable>();

    private PlayerStateManager _stateManager;
    private PlayerAnimatorController _animatorController;
    private PlayerMouseHelper _mouseHelper;
    private PlayerInteraction _playerInteraction;
    private PlayerInputHandler _inputHandler;

    private IPickable _pendingPickable;

    public bool IsHoldingObject => _heldObjects.Count > 0;
    public bool CanPickUp => _heldObjects.Count < _maxPickUpCount && _heldObjects.Count < _holdPoints.Length;
    public int HeldCount => _heldObjects.Count;

    private void Awake()
    {
        _stateManager = GetComponent<PlayerStateManager>();
        _animatorController = GetComponent<PlayerAnimatorController>();
        _mouseHelper = GetComponent<PlayerMouseHelper>();
        _playerInteraction = GetComponent<PlayerInteraction>();
        _inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void OnEnable()
    {
        _playerInteraction.OnInteract += HandleInteract;
        _inputHandler.OnThrowInput += HandleThrowInput;
    }

    private void OnDisable()
    {
        _playerInteraction.OnInteract -= HandleInteract;
        _inputHandler.OnThrowInput -= HandleThrowInput;
    }

    private void HandleThrowInput()
    {
        if (!IsHoldingObject || !CanDoThrow()) return;

        StartThrow();
    }

    private void HandleInteract(IInteractable interactable)
    {
        if (interactable is IPickable pickable)
        {
            if (CanPickUp && CanDoPickUp())
            {
                _pendingPickable = pickable;
                StartPickUp();
            }
        }
    }

    private bool CanDoPickUp()
    {
        return _stateManager.IsInStates(PlayerState.Idle, PlayerState.Moving, PlayerState.PickUp);
    }

    private bool CanDoThrow()
    {
        return _stateManager.IsState(PlayerState.PickUp);
    }

    private void StartPickUp()
    {
        _stateManager.ChangeState(PlayerState.PickUp);
        _animatorController.PickUpAnimation();
    }

    private void StartThrow()
    {
        _mouseHelper.RotateTowardsMouse();
        _stateManager.ChangeState(PlayerState.Throw);
        _animatorController.ThrowAnimation();
    }

    /// <summary>
    /// 애니메이션 이벤트: PickUp 실행
    /// </summary>
    public void OnPickUp()
    {
        if (_pendingPickable != null)
        {
            int holdIndex = _heldObjects.Count;
            if (holdIndex < _holdPoints.Length)
            {
                _pendingPickable.OnPickedUp(_holdPoints[holdIndex]);
                _heldObjects.Add(_pendingPickable);
                _playerInteraction.Remove(_pendingPickable);
                _pendingPickable = null;
            }
        }
    }

    /// <summary>
    /// 애니메이션 이벤트: PickUp 애니메이션 종료
    /// </summary>
    public void OnPickUpAnimationEnd()
    {
        // PickUp 상태 유지 (들고 있는 상태)
    }

    /// <summary>
    /// 애니메이션 이벤트: Throw 실행
    /// </summary>
    public void OnThrow()
    {
        if (_heldObjects.Count > 0)
        {
            // FIFO: 먼저 집은 것부터 던짐
            IPickable pickable = _heldObjects[0];
            _heldObjects.RemoveAt(0);

            Vector3 throwDirection = (transform.forward + Vector3.up * _throwUpwardAngle).normalized;
            pickable.OnThrown(throwDirection, _throwForce);

            // 남은 오브젝트 위치 재정렬
            RearrangeHeldObjects();
        }
    }

    private void RearrangeHeldObjects()
    {
        for (int i = 0; i < _heldObjects.Count; i++)
        {
            if (i < _holdPoints.Length)
            {
                Transform holdPoint = _holdPoints[i];
                _heldObjects[i].Transform.SetParent(holdPoint);
                _heldObjects[i].Transform.localPosition = Vector3.zero;
                _heldObjects[i].Transform.localRotation = Quaternion.identity;
            }
        }
    }

    /// <summary>
    /// 애니메이션 이벤트: Throw 애니메이션 종료
    /// </summary>
    public void OnThrowAnimationEnd()
    {
        // 아직 들고 있는게 있으면 PickUp 상태 유지, 없으면 Idle
        if (_heldObjects.Count > 0)
        {
            _stateManager.ChangeState(PlayerState.PickUp);
            _animatorController.PickUpAnimation();
        }
        else
        {
            _stateManager.ChangeState(PlayerState.Idle);
            _animatorController.ThrowFinishAnimation();
        }
    }
}
