using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 720f;
    [SerializeField] private float _gravity = 20f;

    private CharacterController _controller;
    private Vector3 _moveDirection;

    void Start()
    {
        _controller = GetComponent<CharacterController>();

        if (_controller == null)
        {
            Debug.LogError("CharacterController 컴포넌트가 필요합니다!");
        }
    }

    void Update()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.W)) vertical = 1f;    
        if (Input.GetKey(KeyCode.S)) vertical = -1f;   
        if (Input.GetKey(KeyCode.A)) horizontal = -1f; 
        if (Input.GetKey(KeyCode.D)) horizontal = 1f;  

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical);

        if (inputDirection.magnitude > 0.1f)
        {
            inputDirection.Normalize();

            Quaternion toRotation = Quaternion.LookRotation(inputDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

            _moveDirection.x = inputDirection.x * _moveSpeed;
            _moveDirection.z = inputDirection.z * _moveSpeed;
        }
        else
        {
            _moveDirection.x = 0f;
            _moveDirection.z = 0f;
        }

        if (_controller.isGrounded)
        {
            _moveDirection.y = -0.5f;
        }
        else
        {
            _moveDirection.y -= _gravity * Time.deltaTime;
        }

        _controller.Move(_moveDirection * Time.deltaTime);
    }
}
