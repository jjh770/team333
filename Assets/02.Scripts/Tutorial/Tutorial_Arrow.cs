using DG.Tweening;
using UnityEngine;

public class Tutorial_Arrow : MonoBehaviour
{
    private Tween _arrowMoving;
    private void Start()
    {
        MovingArrow();
    }

    private void MovingArrow()
    {
        if (_arrowMoving != null)
        {
            _arrowMoving.Kill();
        }

        _arrowMoving = transform.DOLocalMoveX(0, 2f).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDestroy()
    {
        _arrowMoving?.Kill();
    }
}
