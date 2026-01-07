using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float _dashSpeed = 20f;
    [SerializeField] private float _dashDuration = 0.2f;
    [SerializeField] private float _dashCooldown = 0.5f;

    private CharacterController _controller;
    private PlayerMove _playerMove;
    private bool _isDashing;
    private float _dashTimer;
    private float _dashCooldownTimer;
    private Vector3 _dashDirection;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _playerMove = GetComponent<PlayerMove>();
    }

    private void Update()
    {
        UpdateDashCooldown();
        HandleDashInput();

        if (_isDashing)
        {
            PerformDash();
        }
    }

    private void UpdateDashCooldown()
    {
        if (_dashCooldownTimer > 0)
        {
            _dashCooldownTimer -= Time.deltaTime;
        }
    }

    private void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _dashCooldownTimer <= 0 && !_isDashing)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

            if (direction.magnitude >= 0.1f)
            {
                StartDash(direction);
            }
        }
    }

    private void StartDash(Vector3 direction)
    {
        _isDashing = true;
        _dashTimer = _dashDuration;
        _dashDirection = direction;
        _dashCooldownTimer = _dashCooldown;

        if (_playerMove != null)
        {
            _playerMove.CanMove = false;
        }
    }

    private void PerformDash()
    {
        _dashTimer -= Time.deltaTime;

        if (_dashTimer <= 0)
        {
            EndDash();
            return;
        }

        Vector3 dashMove = _dashDirection * _dashSpeed * Time.deltaTime;
        _controller.Move(dashMove);
    }

    private void EndDash()
    {
        _isDashing = false;

        if (_playerMove != null)
        {
            _playerMove.CanMove = true;
        }
    }

    public bool IsDashing => _isDashing;
    public float CooldownRemaining => _dashCooldownTimer;
}
