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

    public void DashAnimation(bool isDashing)
    {
        //_animator.SetBool("")
    }
}
