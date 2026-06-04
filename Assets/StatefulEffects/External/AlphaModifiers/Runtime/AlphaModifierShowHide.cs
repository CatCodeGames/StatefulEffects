#if STATEFULEFFECTS_ALPHAMODIFIERS_SUPPORT
#if STATEFULEFFECTS_DOTWEEN_SUPPORT

using CatCode.AlphaModifiers;
using DG.Tweening;
using System;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    public sealed class AlphaModifierShowHide : CallbackMonoShowHide
    {
        [Serializable]
        public struct AlphaEffectSettings
        {
            public float Alpha;
            public float Duration;
            public Ease Ease;
            public bool UseCurve;
            public AnimationCurve Curve;
        }

        private Tween _tween;

        [SerializeField] private MonoAlphaModifier _alphaModifier;
        [SerializeField] private AlphaEffectSettings _showSettings;
        [SerializeField] private AlphaEffectSettings _hideSettings;
        [SerializeField] private bool _isIndependentUpdate;

        private void Awake()
        {
            Initialize();
        }

    

        protected override void OnShow(Action onCompleted) => ChangeAlpha(_showSettings, onCompleted);
        protected override void OnSetShown() => SetAlpha(_showSettings);
        protected override void OnHide(Action onCompleted) => ChangeAlpha(_hideSettings, onCompleted);
        protected override void OnSetHidden() => SetAlpha(_hideSettings);
        protected override void OnStop() => _tween.Kill();

        private void ChangeAlpha(AlphaEffectSettings settings, Action callback)
        {
            _tween.Kill();
            _tween = DOTween
                .To(() => _alphaModifier.Alpha, value => _alphaModifier.Alpha = value, settings.Alpha, settings.Duration)
                .SetUpdate(_isIndependentUpdate)
                .OnComplete(() => callback());

            if (settings.UseCurve)
                _tween.SetEase(settings.Curve);
            else
                _tween.SetEase(settings.Ease);
        }

        private void SetAlpha(AlphaEffectSettings settings)
        {
            _tween.Kill();
            _alphaModifier.Alpha = settings.Alpha;
        }
    }
}
#endif
#endif