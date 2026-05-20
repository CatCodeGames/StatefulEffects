using DG.Tweening;
using System;

namespace CatCode.StatefulEffects
{
    public static class FloatTweenUtility
    {
        public static Tween Play(
            float currentValue,
            FloatEffectSettings from,
            FloatEffectSettings to,
            TweenCallback<float> onUpdate)
        {

            var fromValue = from.Value;
            var toValue = to.Value;

            var t = MathF.Abs((toValue - currentValue) / (toValue - fromValue));
            var duration = t * to.Duration;

            var tween = DOVirtual.Float(currentValue, toValue, duration, onUpdate)
                .SetUpdate(true);

            if (to.Ease.UseCurve)
                tween.SetEase(to.Ease.Curve);
            else
                tween.SetEase(to.Ease.Ease);

            return tween;
        }
    }
}