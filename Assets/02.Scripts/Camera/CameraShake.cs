using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("Default Settings")]
    [SerializeField] private float _defaultDuration = 0.15f;
    [SerializeField] private float _defaultMagnitude = 0.25f;

    private Coroutine _shakeCoroutine;

    public Vector3 ShakeOffset { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Shake()
    {
        Shake(_defaultDuration, _defaultMagnitude);
    }

    public void Shake(float duration, float magnitude)
    {
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
            ShakeOffset = Vector3.zero;
        }

        _shakeCoroutine = StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            ShakeOffset = new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        ShakeOffset = Vector3.zero;
        _shakeCoroutine = null;
    }
}
