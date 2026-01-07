using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MonsterStateController : MonoBehaviour, IAnimationStateChanger
{
    private Animator _animator;
    private MonsterState _currentState = MonsterState.Idle;

    private static readonly int s_animationHash = Animator.StringToHash("animation");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void ChangeState(MonsterState newState)
    {
        if (_currentState == newState) return;

        _currentState = newState;
        _animator.SetInteger(s_animationHash, (int)_currentState);
    }
}
