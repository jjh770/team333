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

    /// <summary>
    /// 현재 재생 중인 공격 애니메이션의 길이를 가져옴
    /// </summary>
    public float GetCurrentAttackAnimationDuration()
    {
        if (_animator == null)
            return 0.5f; // 기본값

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        // 애니메이션 클립 길이 반환 (애니메이터 속도 고려)
        float clipLength = stateInfo.length;
        float animatorSpeed = _animator.speed;

        return clipLength / animatorSpeed;
    }
}
