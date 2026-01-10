using System;
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera _camera;
    [SerializeField] private FloraFollowCamera _followCamera;
    [SerializeField] private Transform _flora;

    [Header("Intro Settings")]
    [SerializeField] private Transform _introTransform;
    [SerializeField] private float _introHoldDuration = 3f; 
    [SerializeField] private float _introMoveDuration = 2f; 
    [SerializeField] private AnimationCurve _introEaseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Outro Settings")]
    [SerializeField] private Transform _outroTransform;
    [SerializeField] private float _outroMoveDuration = 2f; 
    [SerializeField] private float _outroHoldDuration = 3f; 
    [SerializeField] private AnimationCurve _outroEaseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Coroutine _transitionCoroutine;

    public event Action OnIntroComplete;
    public event Action OnOutroComplete;

    private void Awake()
    {
        if (_camera == null)
            _camera = GetComponent<Camera>();
        
        if (_followCamera != null)
        {
            _followCamera.enabled = false;
        }
    }

    public void StartIntro()
    {
        if (_transitionCoroutine != null)
            StopCoroutine(_transitionCoroutine);
        
        if (_followCamera != null)
        {
            _followCamera.enabled = false;
        }

        if (_introTransform != null)
        {
            transform.position = _introTransform.position;
            transform.rotation = _introTransform.rotation;
        }

        _transitionCoroutine = StartCoroutine(IntroTransition());
    }

    public void StartPlaying()
    {
        if (_transitionCoroutine != null)
            StopCoroutine(_transitionCoroutine);

        if (_followCamera != null)
        {
            _followCamera.enabled = true;
        }
    }

    public void StartOutro()
    {
        if (_transitionCoroutine != null)
            StopCoroutine(_transitionCoroutine);

        if (_followCamera != null)
        {
            _followCamera.enabled = false;
        }

        _transitionCoroutine = StartCoroutine(OutroTransition());
    }

    private IEnumerator IntroTransition()
    {
        if (_flora == null || _followCamera == null || _introTransform == null)
        {
            yield break;
        }

        yield return new WaitForSeconds(_introHoldDuration);

        Vector3 startPos = _introTransform.position;
        Quaternion startRot = _introTransform.rotation;

        Vector3 targetPos = _flora.position + _flora.TransformDirection(_followCamera.GetLocalOffset());
        Quaternion targetRot = Quaternion.LookRotation(_flora.position - targetPos);

        float elapsed = 0f;
        while (elapsed < _introMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = _introEaseCurve.Evaluate(elapsed / _introMoveDuration);

            targetPos = _flora.position + _flora.TransformDirection(_followCamera.GetLocalOffset());
            targetRot = Quaternion.LookRotation(_flora.position - targetPos);

            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;

        OnIntroComplete?.Invoke();
        GameStateManager.Instance?.OnIntroComplete();
    }

    private IEnumerator OutroTransition()
    {
        if (_flora == null || _outroTransform == null)
        {
            yield break;
        }

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 targetPos = _outroTransform.position;
        Quaternion targetRot = _outroTransform.rotation;

        float elapsed = 0f;
        while (elapsed < _outroMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = _outroEaseCurve.Evaluate(elapsed / _outroMoveDuration);

            if (_outroTransform != null)
            {
                targetPos = _outroTransform.position;
                targetRot = _outroTransform.rotation;
            }

            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;

        yield return new WaitForSeconds(_outroHoldDuration);

        OnOutroComplete?.Invoke();
        GameStateManager.Instance?.OnOutroComplete();
    }
}