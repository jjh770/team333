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
        if (_animator == null)
            return 0.5f;

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        float clipLength = stateInfo.length;
        float animatorSpeed = _animator.speed;

        return clipLength / animatorSpeed;
    }
}
