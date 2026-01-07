using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _runSpeed = 8f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _rotationSpeed = 10f;

    private PlayerAnimatorController _animatorController;
    private CharacterController _controller;
    private Vector3 _velocity;

    public bool CanMove { get; set; } = true;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animatorController = GetComponent<PlayerAnimatorController>();
    }

    private void Update()
    {
        if (CanMove)
        {
            HandleMovement();
        }

        ApplyGravity();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? _runSpeed : _moveSpeed;
            Vector3 move = direction * currentSpeed * Time.deltaTime;
            _animatorController.MoveAnimation(true);
            _controller.Move(move);

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
        else
        {
            _animatorController.MoveAnimation(false);
        }
    }

    private void ApplyGravity()
    {
        if (_controller.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }
}
