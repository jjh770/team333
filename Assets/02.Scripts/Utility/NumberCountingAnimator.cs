using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

/// <summary>
/// 숫자를 카운팅하는 애니메이션을 제공하는 유틸리티 클래스
/// </summary>
public static class NumberCountingAnimator
{
    /// <summary>
    /// TMP_Text에 숫자 카운팅 애니메이션을 적용합니다.
    /// </summary>
    /// <param name="text">애니메이션을 적용할 TMP_Text</param>
    /// <param name="targetValue">목표 숫자</param>
    /// <param name="duration">애니메이션 지속 시간</param>
    /// <param name="startValue">시작 숫자 (기본값: 0)</param>
    /// <param name="ease">이징 타입 (기본값: OutQuad)</param>
    /// <param name="format">숫자 포맷 (기본값: null, 정수로 표시)</param>
    /// <param name="onComplete">완료 시 콜백</param>
    /// <returns>생성된 Tween (취소용)</returns>
    public static Tween CountTo(
        TMP_Text text,
        int targetValue,
        float duration = 1f,
        int startValue = 0,
        Ease ease = Ease.OutQuad,
        string format = null,
        Action onComplete = null,
        Action<int> onValueChanged = null)
    {
        if (text == null) return null;

        int currentValue = startValue;
        int lastValue = startValue;
        text.text = FormatNumber(currentValue, format);
        onValueChanged?.Invoke(currentValue);

        return DOTween.To(
            () => currentValue,
            x =>
            {
                currentValue = x;
                if (currentValue != lastValue)
                {
                    lastValue = currentValue;
                    text.text = FormatNumber(currentValue, format);
                    onValueChanged?.Invoke(currentValue);
                }
            },
            targetValue,
            duration)
            .SetEase(ease)
            .OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// TMP_Text에 float 숫자 카운팅 애니메이션을 적용합니다.
    /// </summary>
    public static Tween CountTo(
        TMP_Text text,
        float targetValue,
        float duration = 1f,
        float startValue = 0f,
        Ease ease = Ease.OutQuad,
        string format = "F2",
        Action onComplete = null)
    {
        if (text == null) return null;

        float currentValue = startValue;
        text.text = currentValue.ToString(format);

        return DOTween.To(
            () => currentValue,
            x =>
            {
                currentValue = x;
                text.text = currentValue.ToString(format);
            },
            targetValue,
            duration)
            .SetEase(ease)
            .OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// 딜레이 후 숫자 카운팅 애니메이션을 시작합니다.
    /// </summary>
    public static Sequence CountToWithDelay(
        TMP_Text text,
        int targetValue,
        float delay,
        float duration = 1f,
        int startValue = 0,
        Ease ease = Ease.OutQuad,
        string format = null,
        Action onComplete = null,
        Action<int> onValueChanged = null)
    {
        if (text == null) return null;

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(delay);
        seq.AppendCallback(() => CountTo(text, targetValue, duration, startValue, ease, format, onComplete, onValueChanged));

        return seq;
    }

    /// <summary>
    /// 스케일 펀치 효과와 함께 숫자 카운팅 애니메이션을 적용합니다.
    /// </summary>
    public static Sequence CountToWithPunch(
        TMP_Text text,
        int targetValue,
        float duration = 1f,
        int startValue = 0,
        Ease ease = Ease.OutQuad,
        string format = null,
        float punchScale = 0.2f,
        Action onComplete = null)
    {
        if (text == null) return null;

        Sequence seq = DOTween.Sequence();

        int currentValue = startValue;
        text.text = FormatNumber(currentValue, format);

        seq.Append(DOTween.To(
            () => currentValue,
            x =>
            {
                currentValue = x;
                text.text = FormatNumber(currentValue, format);
            },
            targetValue,
            duration)
            .SetEase(ease));

        seq.Join(text.transform.DOPunchScale(Vector3.one * punchScale, duration, 5, 0.5f));
        seq.OnComplete(() => onComplete?.Invoke());

        return seq;
    }

    /// <summary>
    /// 커스텀 포맷터를 사용한 float 카운팅 애니메이션
    /// </summary>
    public static Tween CountTo(
        TMP_Text text,
        float targetValue,
        Func<float, string> formatter,
        float duration = 1f,
        float startValue = 0f,
        Ease ease = Ease.OutQuad,
        Action onComplete = null)
    {
        if (text == null) return null;

        float currentValue = startValue;
        text.text = formatter(currentValue);

        return DOTween.To(
            () => currentValue,
            x =>
            {
                currentValue = x;
                text.text = formatter(currentValue);
            },
            targetValue,
            duration)
            .SetEase(ease)
            .OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// 딜레이 후 커스텀 포맷터를 사용한 float 카운팅 애니메이션
    /// </summary>
    public static Sequence CountToWithDelay(
        TMP_Text text,
        float targetValue,
        Func<float, string> formatter,
        float delay,
        float duration = 1f,
        float startValue = 0f,
        Ease ease = Ease.OutQuad,
        Action onComplete = null)
    {
        if (text == null) return null;

        text.text = formatter(startValue);

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(delay);
        seq.AppendCallback(() => CountTo(text, targetValue, formatter, duration, startValue, ease, onComplete));

        return seq;
    }

    /// <summary>
    /// 시간 형식(MM:SS.ms)으로 카운팅 애니메이션을 적용합니다.
    /// </summary>
    public static Tween CountToTime(
        TMP_Text text,
        float targetSeconds,
        float duration = 1f,
        float startSeconds = 0f,
        Ease ease = Ease.OutQuad,
        Action onComplete = null)
    {
        return CountTo(text, targetSeconds, FormatTime, duration, startSeconds, ease, onComplete);
    }

    /// <summary>
    /// 딜레이 후 시간 형식(MM:SS.ms)으로 카운팅 애니메이션을 시작합니다.
    /// </summary>
    public static Sequence CountToTimeWithDelay(
        TMP_Text text,
        float targetSeconds,
        float delay,
        float duration = 1f,
        float startSeconds = 0f,
        Ease ease = Ease.OutQuad,
        Action onComplete = null)
    {
        return CountToWithDelay(text, targetSeconds, FormatTime, delay, duration, startSeconds, ease, onComplete);
    }

    private static string FormatTime(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        return $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}.{timeSpan.Milliseconds / 10:00}";
    }

    private static string FormatNumber(int value, string format)
    {
        if (string.IsNullOrEmpty(format))
        {
            return value.ToString();
        }
        return value.ToString(format);
    }
}
