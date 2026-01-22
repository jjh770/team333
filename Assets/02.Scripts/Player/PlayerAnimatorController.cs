using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int Dash = Animator.StringToHash("Dash");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int AttackCombo = Animator.StringToHash("AttackCombo");
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int PickUp = Animator.StringToHash("PickUp");
    private static readonly int Throw = Animator.StringToHash("Throw");
    private static readonly int ThrowFinish = Animator.StringToHash("ThrowFinish");
    private static readonly int Skill = Animator.StringToHash("Skill");
    private static readonly int Clear = Animator.StringToHash("Clear");

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void MoveAnimation(bool isMoving)
    {
        _animator.SetBool(IsMoving, isMoving);
    }

    public void DashAnimation()
    {
        _animator.SetTrigger(Dash);
    }

    public void AttackAnimation(int comboAttackNum)
    {
        _animator.SetTrigger(Attack);
        _animator.SetInteger(AttackCombo, comboAttackNum);
    }

    public void DieAnimation()
    {
        _animator.SetTrigger(Die);
    }

    public void ClearAnimation()
    {
        _animator.SetTrigger(Clear);
    }

    public void PickUpAnimation()
    {
        _animator.SetTrigger(PickUp);
    }

    public void ThrowAnimation()
    {
        _animator.SetTrigger(Throw);
    }

    public void ThrowFinishAnimation()
    {
        _animator.SetTrigger(ThrowFinish);
    }

    public void SkillAnimation()
    {
        _animator.SetTrigger(Skill);
    }

    public float GetCurrentAnimationDuration()
    {
        if (_animator == null) return 0.5f;

        // 현재 전이 중이라면 다음 상태의 정보를 가져오도록 시도
        AnimatorStateInfo stateInfo = _animator.IsInTransition(0) 
            ? _animator.GetNextAnimatorStateInfo(0) 
            : _animator.GetCurrentAnimatorStateInfo(0);

        float clipLength = stateInfo.length;
        float animatorSpeed = _animator.speed;
        
        Debug.Log(clipLength / animatorSpeed);
        
        return clipLength > 0 ? (clipLength / animatorSpeed) : 0.5f;
    }
}
