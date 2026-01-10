using UnityEngine;

public class MonsterSpeechBubble : MonoBehaviour
{
    [SerializeField] private RectTransform _speechBubbleTransform;
    private Transform _cameraTransform;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        _speechBubbleTransform.forward = _cameraTransform.forward;
    }
}
