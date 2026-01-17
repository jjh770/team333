using System;
using UnityEngine;

public class PauseInputHandler : MonoBehaviour
{
    [SerializeField] private KeyCode _pauseKey = KeyCode.Escape;

    public static event Action OnPauseToggleRequested;

    private void Update()
    {
        if (Input.GetKeyDown(_pauseKey))
        {
            OnPauseToggleRequested?.Invoke();
        }
    }
}
