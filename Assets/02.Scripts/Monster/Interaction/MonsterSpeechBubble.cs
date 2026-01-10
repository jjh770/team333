using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MonsterSpeechBubble : MonoBehaviour
{
    [SerializeField] private Transform _speechBubbleTransform;
    private Transform _cameraTransform;
    private Tweener _tween;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        _speechBubbleTransform.forward = _cameraTransform.forward;
    }
}
