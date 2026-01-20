using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider _progressBar;
    [SerializeField] private RectTransform _followIcon;

    [Header("Settings")]
    [SerializeField] private float _minLoadingTime = 0.5f;

    private static string _targetSceneName;
    private RectTransform _fillRect;

    public static void LoadScene(string sceneName)
    {
        _targetSceneName = sceneName;
        SceneManager.LoadSceneAsync("LoadingScene");
    }

    private void Start()
    {
        if (_progressBar.fillRect != null)
        {
            _fillRect = _progressBar.fillRect;
        }

        if (string.IsNullOrEmpty(_targetSceneName))
        {
            return;
        }

        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        _progressBar.value = 0f;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_targetSceneName);
        asyncOperation.allowSceneActivation = false;

        float elapsedTime = 0f;

        while (!asyncOperation.isDone)
        {
            elapsedTime += Time.deltaTime;

            // 실제 로딩 진행률 (0 ~ 0.9)
            float realProgress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            // 최소 로딩 시간을 고려한 진행률
            float timeProgress = Mathf.Clamp01(elapsedTime / _minLoadingTime);

            // 둘 중 작은 값 사용 (로딩이 빨라도 최소 시간 보장)
            float displayProgress = Mathf.Min(realProgress, timeProgress);
            _progressBar.value = displayProgress;
            UpdateIconPosition();

            // 로딩 완료 조건: 실제 로딩 완료 + 최소 시간 경과
            if (asyncOperation.progress >= 0.9f && elapsedTime >= _minLoadingTime)
            {
                _progressBar.value = 1f;
                UpdateIconPosition();
                yield return null;
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private void UpdateIconPosition()
    {
        if (_followIcon == null || _fillRect == null) return;

        Vector3[] corners = new Vector3[4];
        _fillRect.GetWorldCorners(corners);

        // corners[2]는 Fill 영역의 오른쪽 상단, corners[3]은 오른쪽 하단
        // Fill의 오른쪽 끝 중앙 위치 계산
        Vector3 rightEdge = (corners[2] + corners[3]) / 2f;
        _followIcon.position = rightEdge;
    }
}
