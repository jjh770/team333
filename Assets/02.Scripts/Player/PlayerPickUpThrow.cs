using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpThrow : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _maxPickUpCount = 2;
    [SerializeField] private Transform[] _holdPoints;
    [SerializeField] private float _throwForce = 10f;
    [SerializeField] private KeyCode _pickUpThrowKey = KeyCode.E;

    [Header("Debug")]
    [SerializeField] private List<IPickable> _heldObjects = new List<IPickable>();
    [SerializeField] private List<IPickable> _nearbyPickables = new List<IPickable>();

    private PlayerStateManager _stateManager;
    private PlayerAnimatorController _animatorController;
    private PlayerRotateToMouse _rotateToMouse;

    public bool IsHoldingObject => _heldObjects.Count > 0;
    public bool CanPickUp => _nearbyPickables.Count > 0 && _heldObjects.Count < _maxPickUpCount && _heldObjects.Count < _holdPoints.Length;
    public int HeldCount => _heldObjects.Count;

    private void Awake()
    {
        _stateManager = GetComponent<PlayerStateManager>();
        _animatorController = GetComponent<PlayerAnimatorController>();
        _rotateToMouse = GetComponent<PlayerRotateToMouse>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(_pickUpThrowKey))
        {
            HandlePickUpThrowInput();
        }
    }

    private void HandlePickUpThrowInput()
    {
        // PickUp이 Throw보다 우선
        if (CanPickUp && CanDoPickUp())
        {
            StartPickUp();
        }
        else if (IsHoldingObject && CanDoThrow())
        {
            StartThrow();
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
        _rotateToMouse.RotateTowardsMouse();
        _stateManager.ChangeState(PlayerState.Throw);
        _animatorController.ThrowAnimation();
    }

    /// <summary>
    /// 애니메이션 이벤트: PickUp 실행
    /// </summary>
    public void OnPickUp()
    {
        if (_nearbyPickables.Count > 0)
        {
            IPickable pickable = GetClosestPickable();
            if (pickable != null)
            {
                int holdIndex = _heldObjects.Count;
                if (holdIndex < _holdPoints.Length)
                {
                    pickable.OnPickedUp(_holdPoints[holdIndex]);
                    _heldObjects.Add(pickable);
                    _nearbyPickables.Remove(pickable);
                }
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

            Vector3 throwDirection = transform.forward + transform.up * 0.5f;
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

    private IPickable GetClosestPickable()
    {
        if (_nearbyPickables.Count == 0) return null;

        IPickable closest = null;
        float closestDistance = float.MaxValue;

        foreach (var pickable in _nearbyPickables)
        {
            if (pickable == null) continue;

            float distance = Vector3.Distance(transform.position, pickable.Transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = pickable;
            }
        }

        return closest;
    }

    private void OnTriggerEnter(Collider other)
    {
        IPickable pickable = other.GetComponent<IPickable>();
        if (pickable != null && !_nearbyPickables.Contains(pickable) && !_heldObjects.Contains(pickable))
        {
            _nearbyPickables.Add(pickable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IPickable pickable = other.GetComponent<IPickable>();
        if (pickable != null && _nearbyPickables.Contains(pickable))
        {
            _nearbyPickables.Remove(pickable);
        }
    }
}
