using UnityEngine;
using UnityEngine.UI;

public class UI_FloraProgress : MonoBehaviour
{
    [SerializeField] private FloraMovement _floraMovement;
    [SerializeField] private Image _progressBar;

    private void OnEnable()
    {
        _floraMovement.OnProgressChanged += UpdateUI;
    }

    private void OnDisable()
    {
        _floraMovement.OnProgressChanged -= UpdateUI;
    }

    private void UpdateUI(float progress)
    {
        _progressBar.fillAmount = progress;
    }
}
