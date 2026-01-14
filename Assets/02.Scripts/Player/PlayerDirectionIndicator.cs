using DG.Tweening;
using UnityEngine;

public class PlayerDirectionIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _indicator;
    private MeshRenderer _indicatorRenderer;
    private PlayerMove _playerMove;

    [Header("Show Settings")]
    [SerializeField] private float _showDuration = 1f;
    [SerializeField] private Ease _showEase;

    [Header("Rotation Settings")]
    [SerializeField] private float _rotationSmoothTime = 0.1f;

    private PlayerStateManager _stateManager;
    private float _rotationVelocity;
    private Tweener _showTween;
    private float _targetAngle;
    private float _currentAngle;

    private void Awake()
    {
        _stateManager = GetComponent<PlayerStateManager>();
        _playerMove = GetComponent<PlayerMove>();
        _indicatorRenderer = _indicator.GetComponent<MeshRenderer>();
        _indicator.transform.localScale = Vector3.zero;
    }

    private void Start()
    {
        _stateManager.OnPlayState += HandleIndicator;
        _stateManager.OnStateChanged += HandleStateChanged;

        if (GameStateManager.Instance != null)
        {
            this.enabled = GameStateManager.Instance.IsPlaying;
        }
    }

    private void OnDestroy()
    {
        _stateManager.OnPlayState -= HandleIndicator;
        _stateManager.OnStateChanged -= HandleStateChanged;
        _showTween?.Kill();
    }

    private void Update()
    {
        Vector3 direction = _playerMove.GetMovementDirection();

        if (direction != Vector3.zero)
        {
            _targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        }

        _currentAngle = Mathf.SmoothDampAngle(_currentAngle, _targetAngle, ref _rotationVelocity, _rotationSmoothTime);
        _indicator.transform.rotation = Quaternion.Euler(0f, _currentAngle, 0f);
    }

    private void HandleIndicator(bool isPlaying)
    {
        this.enabled = isPlaying;
        if (!isPlaying)
        {
            _indicatorRenderer.enabled = false;
        }
        else
        {
            ShowIn();
        }
    }

    private void HandleStateChanged(PlayerState previousState, PlayerState newState)
    {
        if (newState == PlayerState.Die)
        {
            ShowOut();
        }
    }

    private void ShowIn()
    {
        _showTween?.Kill();
        _indicatorRenderer.enabled = true;
        _showTween = _indicator.transform.DOScale(Vector3.one, _showDuration).SetEase(_showEase);
    }

    private void ShowOut()
    {
        _showTween?.Kill();
        _showTween = _indicator.transform.DOScale(Vector3.zero, _showDuration)
            .SetEase(_showEase)
            .OnComplete(() => _indicatorRenderer.enabled = false);
    }
}
