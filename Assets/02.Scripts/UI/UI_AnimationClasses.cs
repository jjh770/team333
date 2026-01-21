using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI.Animations
{
    [System.Serializable]
    public class UIElementAnimation
    {
        public RectTransform element;
        public Vector2 hiddenPosition;
        public Vector2 visiblePosition;
        [Tooltip("애니메이션 지속 시간")]
        public float duration = 1f;
        public Ease ease = Ease.OutQuad;

        public void SetToHidden()
        {
            if (element != null)
            {
                element.anchoredPosition = hiddenPosition;
            }
        }

        public void AnimateToVisible()
        {
            if (element != null)
            {
                element.DOAnchorPos(visiblePosition, duration).SetEase(ease).SetUpdate(true);
            }
        }

        public void AnimateToHidden()
        {
            if (element != null)
            {
                element.DOAnchorPos(hiddenPosition, duration).SetEase(ease).SetUpdate(true);
            }
        }

        public void AnimateToHidden(Action onComplete)
        {
            if (element != null)
            {
                element.DOAnchorPos(hiddenPosition, duration).SetEase(ease).SetUpdate(true).OnComplete(() => onComplete?.Invoke());
            }
        }
    }

    [System.Serializable]
    public class UIFadeAnimation
    {
        public Graphic graphic;
        public float hiddenAlpha = 0f;
        public float visibleAlpha = 1f;
        [Tooltip("애니메이션 지속 시간")]
        public float duration = 1f;
        public Ease ease = Ease.OutQuad;

        public void SetToHidden()
        {
            if (graphic != null)
            {
                Color color = graphic.color;
                color.a = hiddenAlpha;
                graphic.color = color;
            }
        }

        public void AnimateToVisible()
        {
            if (graphic != null)
            {
                graphic.DOFade(visibleAlpha, duration).SetEase(ease).SetUpdate(true);
            }
        }

        public void AnimateToHidden()
        {
            if (graphic != null)
            {
                graphic.DOFade(hiddenAlpha, duration).SetEase(ease).SetUpdate(true);
            }
        }

        public void AnimateToHidden(Action onComplete)
        {
            if (graphic != null)
            {
                graphic.DOFade(hiddenAlpha, duration).SetEase(ease).SetUpdate(true).OnComplete(() => onComplete?.Invoke());
            }
        }
    }

    [System.Serializable]
    public class UIElementDelayAnimation
    {
        public RectTransform element;
        public Vector2 hiddenPosition;
        public Vector2 visiblePosition;
        [Tooltip("애니메이션 시작 전 대기 시간")]
        public float delay = 0f;
        [Tooltip("애니메이션 지속 시간")]
        public float duration = 1f;
        public Ease ease = Ease.OutQuad;

        public void SetToHidden()
        {
            if (element != null)
            {
                element.anchoredPosition = hiddenPosition;
            }
        }

        public void AnimateToVisible()
        {
            if (element != null)
            {
                element.DOAnchorPos(visiblePosition, duration).SetDelay(delay).SetEase(ease).SetUpdate(true);
            }
        }

        public void AnimateToVisible(Action onComplete)
        {
            if (element != null)
            {
                element.DOAnchorPos(visiblePosition, duration).SetDelay(delay).SetEase(ease).SetUpdate(true).OnComplete(() => onComplete?.Invoke());
            }
        }

        public void AnimateToHidden()
        {
            if (element != null)
            {
                element.DOAnchorPos(hiddenPosition, duration).SetDelay(delay).SetEase(ease).SetUpdate(true);
            }
        }

        public void AnimateToHidden(Action onComplete)
        {
            if (element != null)
            {
                element.DOAnchorPos(hiddenPosition, duration).SetDelay(delay).SetEase(ease).SetUpdate(true).OnComplete(() => onComplete?.Invoke());
            }
        }
    }

    [System.Serializable]
    public class UIScaleAnimation
    {
        public RectTransform element;
        public Vector3 hiddenScale = Vector3.zero;
        public Vector3 visibleScale = Vector3.one;
        [Tooltip("애니메이션 지속 시간")]
        public float duration = 0.3f;
        public Ease ease = Ease.OutBack;

        public void SetToHidden()
        {
            if (element != null)
            {
                element.localScale = hiddenScale;
            }
        }

        public void SetToVisible()
        {
            if (element != null)
            {
                element.localScale = visibleScale;
            }
        }

        public void AnimateToVisible()
        {
            if (element != null)
            {
                element.DOScale(visibleScale, duration).SetEase(ease).SetUpdate(true);
            }
        }

        public void AnimateToVisible(Action onComplete)
        {
            if (element != null)
            {
                element.DOScale(visibleScale, duration).SetEase(ease).SetUpdate(true).OnComplete(() => onComplete?.Invoke());
            }
        }

        public void AnimateToHidden()
        {
            if (element != null)
            {
                element.DOScale(hiddenScale, duration).SetEase(ease).SetUpdate(true);
            }
        }

        public void AnimateToHidden(Action onComplete)
        {
            if (element != null)
            {
                element.DOScale(hiddenScale, duration).SetEase(ease).SetUpdate(true).OnComplete(() => onComplete?.Invoke());
            }
        }
    }

    [System.Serializable]
    public class UISizeAnimation
    {
        public RectTransform element;
        public Vector2 hiddenSize;
        public Vector2 visibleSize;
        [Tooltip("애니메이션 지속 시간")]
        public float duration = 0.3f;
        public Ease ease = Ease.OutQuad;

        public void SetToHidden()
        {
            if (element != null)
            {
                element.sizeDelta = hiddenSize;
            }
        }

        public void SetToVisible()
        {
            if (element != null)
            {
                element.sizeDelta = visibleSize;
            }
        }

        public void AnimateToVisible()
        {
            if (element != null)
            {
                element.DOSizeDelta(visibleSize, duration).SetEase(ease).SetUpdate(true);
            }
        }

        public void AnimateToVisible(Action onComplete)
        {
            if (element != null)
            {
                element.DOSizeDelta(visibleSize, duration).SetEase(ease).SetUpdate(true).OnComplete(() => onComplete?.Invoke());
            }
        }

        public void AnimateToHidden()
        {
            if (element != null)
            {
                element.DOSizeDelta(hiddenSize, duration).SetEase(ease).SetUpdate(true);
            }
        }

        public void AnimateToHidden(Action onComplete)
        {
            if (element != null)
            {
                element.DOSizeDelta(hiddenSize, duration).SetEase(ease).SetUpdate(true).OnComplete(() => onComplete?.Invoke());
            }
        }
    }
}
