using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void MoveAnimation(bool isMoving)
    {
        _animator.SetBool("IsMoving", isMoving);
    }

    public void DashAnimation()
    {
        _animator.SetTrigger("Dash");
    }

    public void AttackAnimation(int comboAttackNum)
    {
        _animator.SetTrigger("Attack");
        _animator.SetInteger("AttackCombo", comboAttackNum);
    }

    public void DieAnimation()
    {
        _animator.SetTrigger("Die");
    }

    public void PickUpAnimation()
    {
        _animator.SetTrigger("PickUp");
    }

    public void ThrowAnimation()
    {
        _animator.SetTrigger("Throw");
    }

    public void ThrowFinishAnimation()
    {
        _animator.SetTrigger("ThrowFinish");
    }

    public float GetCurrentAttackAnimationDuration()
    {
        if (_animator == null)
            return 0.5f;

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        float clipLength = stateInfo.length;
        float animatorSpeed = _animator.speed;

        return clipLength / animatorSpeed;
    }
}
