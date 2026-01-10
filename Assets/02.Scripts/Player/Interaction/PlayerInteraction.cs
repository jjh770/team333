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
        InteractionType.Use,
        InteractionType.PickUp
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

        // Interact 중 객체가 파괴될 수 있으므로 미리 참조 저장
        IInteractable interactable = _closestInteractable;
        interactable.Interact(gameObject);
        OnInteract?.Invoke(interactable);
    }

    /// <summary>
    /// 우선순위에 따라 가장 가까운 상호작용 가능한 오브젝트 반환
    /// </summary>
    private IInteractable GetClosestInteractableByPriority()
    {
        CleanupNull();
        if (_nearbyInteractables.Count == 0) return null;

        IInteractable bestMatch = null;
        int bestPriorityIndex = int.MaxValue;
        float bestDistanceSqr = float.MaxValue;

        Vector3 playerPos = transform.position;

        foreach (var interactable in _nearbyInteractables)
        {
            if (!interactable.CanInteract) continue;

            // 우선순위 배열에서 현재 인덱스 찾기 (Dictionary 등으로 캐싱하면 더 빠름)
            int priorityIndex = Array.IndexOf(_priorityOrder, interactable.Type);
            if (priorityIndex == -1) continue;

            float distSqr = (interactable.Transform.position - playerPos).sqrMagnitude;

            // 1. 우선순위가 더 높거나 2. 우선순위는 같은데 거리가 더 가까운 경우 교체
            if (priorityIndex < bestPriorityIndex || (priorityIndex == bestPriorityIndex && distSqr < bestDistanceSqr))
            {
                bestPriorityIndex = priorityIndex;
                bestDistanceSqr = distSqr;
                bestMatch = interactable;
            }
        }
        return bestMatch;
    }

    ///// <summary>
    ///// 특정 타입 중에서 가장 가까운 상호작용 가능한 오브젝트 반환
    ///// </summary>
    //private IInteractable GetClosestOfType(InteractionType type)
    //{
    //    IInteractable closest = null;
    //    float closestDistanceSqr = float.MaxValue;

    //    Vector3 playerPos = transform.position;

    //    foreach (var interactable in _nearbyInteractables)
    //    {
    //        if (interactable == null || interactable.Type != type)
    //            continue;

    //        if (!interactable.CanInteract)
    //            continue;

    //        float distanceSqr = (interactable.Transform.position - playerPos).sqrMagnitude;

    //        if (distanceSqr < closestDistanceSqr)
    //        {
    //            closestDistanceSqr = distanceSqr;
    //            closest = interactable;
    //        }
    //    }

    //    return closest;
    //}

    private void CleanupNull()
    {
        _nearbyInteractables.RemoveAll(i =>
        {
            // 1. 인터페이스 참조를 UnityEngine.Object로 캐스팅
            // 유니티의 == null 연산자가 파괴된 객체를 true로 판정해줍니다.
            if (i is UnityEngine.Object obj)
            {
                return obj == null;
            }

            // 2. 만약 유니티 오브젝트가 아닌 일반 C# 클래스라면 단순 null 체크
            return i == null;
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
        var interactables = other.GetComponents<IInteractable>();
        foreach (var interactable in interactables)
        {
            if (!_nearbyInteractables.Contains(interactable))
            {
                _nearbyInteractables.Add(interactable);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var interactables = other.GetComponents<IInteractable>();
        foreach (var interactable in interactables)
        {
            _nearbyInteractables.Remove(interactable);
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
