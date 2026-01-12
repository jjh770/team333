using UnityEngine;
using UnityEngine.UI;

public class UI_FloraProgress : MonoBehaviour
{
    [SerializeField] private FloraMovement _floraMovement;
    [SerializeField] private Image _progressBar;

    private void OnEnable()
    {
        if (_floraMovement != null)
        {
            _floraMovement.OnProgressChanged += UpdateUI;
            UpdateUI(0f);
        }
    }

    private void OnDisable()
    {
        _floraMovement.OnProgressChanged -= UpdateUI;
    }

    private void UpdateUI(float progress)
    {
        if (_progressBar != null)
        {
            _progressBar.fillAmount = progress;
        }
    }
}
