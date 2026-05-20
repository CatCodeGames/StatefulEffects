#if STATEFULEFFECTS_ALPHAMODIFIERS_SUPPORT
#if STATEFULEFFECTS_DOTWEEN_SUPPORT

using CatCode.AlphaModifiers;
using CatCode.Events;
using DG.Tweening;
using System;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    public sealed class AlphaModifierShowHide : MonoShowHide
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

        private bool _initialized;
        private Tween _tween;
        private IShowHide _showHide;

        [SerializeField] private ShowHideState _initialState;
        [SerializeField] private bool _ignoreState;

        [SerializeField] private MonoAlphaModifier _alphaModifier;
        [SerializeField] private AlphaEffectSettings _showSettings;
        [SerializeField] private AlphaEffectSettings _hideSettings;
        [SerializeField] private bool _isIndependentUpdate;

        private void Awake()
        {
            Initialize();
        }

        public override void Initialize()
        {
            if (_initialized)
                return;
            _initialized = true;
            _showHide = new ShowHideCallbackStateMachine(_initialState, _ignoreState, OnShow, OnHide, OnSetShown, OnSetHidden, OnStop);
        }

        private void OnShow(Action onCompleted) => ChangeAlpha(_showSettings, onCompleted);
        private void OnSetShown() => SetAlpha(_showSettings);
        private void OnHide(Action onCompleted) => ChangeAlpha(_hideSettings, onCompleted);
        private void OnSetHidden() => SetAlpha(_hideSettings);
        private void OnStop() => _tween.Kill();

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

        #region IShowHide 

        public override IReadOnlyEventValue<ShowHideState> State => _showHide.State;

        public override void Show() => _showHide.Show();
        public override void SetShown() => _showHide.SetShown();
        public override void Hide() => _showHide.Hide();
        public override void SetHidden() => _showHide.SetHidden();
        public override void Stop() => _showHide.Stop();

        #endregion
    }
}
#endif
#endif