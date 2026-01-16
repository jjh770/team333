using DG.Tweening;
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
                element.DOAnchorPos(visiblePosition, duration);
            }
        }

        public void AnimateToHidden()
        {
            if (element != null)
            {
                element.DOAnchorPos(hiddenPosition, duration);
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
                graphic.DOFade(visibleAlpha, duration);
            }
        }

        public void AnimateToHidden()
        {
            if (graphic != null)
            {
                graphic.DOFade(hiddenAlpha, duration);
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
                element.DOAnchorPos(visiblePosition, duration).SetDelay(delay);
            }
        }

        public void AnimateToHidden()
        {
            if (element != null)
            {
                element.DOAnchorPos(hiddenPosition, duration).SetDelay(delay);
            }
        }
    }
}
