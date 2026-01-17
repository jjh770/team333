using UnityEngine;

public class PauseInputHandler : MonoBehaviour
{
    [SerializeField] private KeyCode _pauseKey = KeyCode.Escape;

    private void Update()
    {
        if (Input.GetKeyDown(_pauseKey))
        {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.TogglePause();
            }
        }
    }
}
