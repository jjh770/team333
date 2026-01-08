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
}
