using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISound : MonoBehaviour
{
    [SerializeField] private AudioClip _buttonClickSound;
    [SerializeField] private float _buttonClickSoundVolume = 1f;

    [SerializeField] private AudioClip _mouseHoverSound;
    [SerializeField] private float _mouseHoverSoundVolume = 0.1f;

    private void Awake()
    {
        var buttons = GetComponentsInChildren<Button>(true);
        foreach (var button in buttons)
        {
            button.onClick.AddListener(PlayButtonSound); // 클릭
            AddHoverSound(button.gameObject); // 호버
        }
    }

    public void AddHoverSound(GameObject target)
    {
        var trigger = target.GetComponent<EventTrigger>();
        if (trigger == null) trigger = target.AddComponent<EventTrigger>();
        if (trigger.triggers == null) trigger.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();

        var entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener(_ => PlayHoverSound());
        trigger.triggers.Add(entry);
    }
    public void PlayButtonSound()
    {
        if (_buttonClickSound == null) return;
        SoundManager.Instance?.PlaySFX(_buttonClickSound, _buttonClickSoundVolume);
    }

    public void PlayHoverSound()
    {
        if (_mouseHoverSound == null) return;
        SoundManager.Instance?.PlaySFX(_mouseHoverSound, _mouseHoverSoundVolume);
    }
}
