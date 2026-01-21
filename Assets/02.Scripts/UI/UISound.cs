using UnityEngine;
using UnityEngine.UI;

public class UISound : MonoBehaviour
{
    [SerializeField] private AudioClip _buttonClickSound;
    [SerializeField] private float _volumeMultiplier = 1f;

    private void Awake()
    {
        var buttons = GetComponentsInChildren<Button>(true);
        foreach (var button in buttons)
        {
            button.onClick.AddListener(PlayButtonSound);
        }
    }

    public void PlayButtonSound()
    {
        if (_buttonClickSound == null) return;
        SoundManager.Instance?.PlaySFX(_buttonClickSound, _volumeMultiplier);
    }
}
