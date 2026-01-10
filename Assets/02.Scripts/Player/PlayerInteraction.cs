using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private KeyCode _interactKey = KeyCode.E;
    [SerializeField] private float _updateInterval = 0.1f;

    private List<IInteractable> _nearbyInteractables = new List<IInteractable>();
    private IInteractable _closestInteractable;
    private float _nextUpdateTime;

    // 우선순위: Move > SpeedUp > PickUp > Use
    private static readonly InteractionType[] _priorityOrder = new InteractionType[]
    {
        InteractionType.TalkToMove,
        InteractionType.TalkToSpeedUp,
        InteractionType.PickUp,
        InteractionType.Use
    };

    public IInteractable ClosestInteractable => _closestInteractable;
    public bool HasInteractable => _closestInteractable != null;
    public InteractionType? CurrentInteractionType => _closestInteractable?.Type;

    public event Action<IInteractable> OnInteractableChanged;
    public event Action<IInteractable> OnInteract;

    private void OnEnable()
    {
        InteractionEvents.OnInteractableDestroyed += HandleInteractableDestroyed;
    }

    private void OnDisable()
    {
        InteractionEvents.OnInteractableDestroyed -= HandleInteractableDestroyed;
    }

    private void HandleInteractableDestroyed(IInteractable interactable)
    {
        Remove(interactable);
    }

    private void Update()
    {
        if (Time.time >= _nextUpdateTime)
        {
            UpdateClosestInteractable();
            _nextUpdateTime = Time.time + _updateInterval;
        }

        if (Input.GetKeyDown(_interactKey) && _closestInteractable != null)
        {
            TryInteract();
        }
    }

    private void UpdateClosestInteractable()
    {
        IInteractable previous = _closestInteractable;
        _closestInteractable = GetClosestInteractableByPriority();

        if (previous != _closestInteractable)
        {
            OnInteractableChanged?.Invoke(_closestInteractable);
        }
    }

    private void TryInteract()
    {
        if (_closestInteractable == null || !_closestInteractable.CanInteract)
            return;

        _closestInteractable.Interact(gameObject);
        OnInteract?.Invoke(_closestInteractable);
    }

    /// <summary>
    /// 우선순위에 따라 가장 가까운 상호작용 가능한 오브젝트 반환
    /// </summary>
    private IInteractable GetClosestInteractableByPriority()
    {
        CleanupNull();

        if (_nearbyInteractables.Count == 0) return null;

        // 우선순위대로 순회
        foreach (var priorityType in _priorityOrder)
        {
            IInteractable closest = GetClosestOfType(priorityType);
            if (closest != null)
            {
                return closest;
            }
        }

        return null;
    }

    /// <summary>
    /// 특정 타입 중에서 가장 가까운 상호작용 가능한 오브젝트 반환
    /// </summary>
    private IInteractable GetClosestOfType(InteractionType type)
    {
        IInteractable closest = null;
        float closestDistanceSqr = float.MaxValue;

        Vector3 playerPos = transform.position;

        foreach (var interactable in _nearbyInteractables)
        {
            if (interactable == null || interactable.Type != type)
                continue;

            if (!interactable.CanInteract)
                continue;

            float distanceSqr = (interactable.Transform.position - playerPos).sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closest = interactable;
            }
        }

        return closest;
    }

    private void CleanupNull()
    {
        _nearbyInteractables.RemoveAll(i =>
        {
            if (i == null) return true;
            if (i is MonoBehaviour mb && mb == null) return true;

            try
            {
                return i.Transform == null;
            }
            catch
            {
                return true;
            }
        });
    }

    public void Remove(IInteractable interactable)
    {
        _nearbyInteractables.Remove(interactable);

        // 제거된 것이 현재 선택된 것이면 다시 업데이트
        if (_closestInteractable == interactable)
        {
            UpdateClosestInteractable();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (var component in other.GetComponents<MonoBehaviour>())
        {
            if (component is IInteractable interactable &&
                !_nearbyInteractables.Contains(interactable))
            {
                _nearbyInteractables.Add(interactable);
                break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (var component in other.GetComponents<MonoBehaviour>())
        {
            if (component is IInteractable interactable)
            {
                _nearbyInteractables.Remove(interactable);
                break;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_nearbyInteractables == null) return;

        foreach (var interactable in _nearbyInteractables)
        {
            if (interactable?.Transform == null) continue;

            // 현재 선택된 것은 녹색, 나머지는 타입별 색상
            if (interactable == _closestInteractable)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(interactable.Transform.position, 0.7f);
            }
            else
            {
                Gizmos.color = GetColorByType(interactable.Type);
                Gizmos.DrawWireSphere(interactable.Transform.position, 0.5f);
            }
        }
    }

    private Color GetColorByType(InteractionType type)
    {
        return type switch
        {
            InteractionType.TalkToMove => Color.cyan,
            InteractionType.TalkToSpeedUp => Color.green,
            InteractionType.PickUp => Color.blue,
            InteractionType.Use => Color.yellow,
            _ => Color.gray
        };
    }
#endif
}
