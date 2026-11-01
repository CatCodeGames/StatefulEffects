using DG.Tweening;
using System;

namespace CatCode.StatefulEffects
{
    public sealed class DOTweenFloatShowHide : ShowHide
    {
        private Tween _tween;

        private readonly TweenCallback<float> _onUpdate;
        private readonly Action<float> _setValue;

        private readonly DOTweenFloatSettings _showSettings;
        private readonly DOTweenFloatSettings _hideSettings;
        private readonly bool _isIndependentUpdate;
        private readonly ShowHidePolicy _policy;

        public float Value { get; private set; }

        protected override ShowHidePolicy Policy => _policy;

        public DOTweenFloatShowHide(
            Action<float> setValue,
            DOTweenFloatSettings showSettings,
            DOTweenFloatSettings hideSettings,
            bool isIndependentUpdate = false,
            ShowHidePolicy policy = default)
        {
            _policy = policy;
            _setValue = setValue;

            _showSettings = showSettings;
            _hideSettings = hideSettings;
            _isIndependentUpdate = isIndependentUpdate;

            _onUpdate = SetValue;
        }

        protected override void OnShow(ShowHideState from, Action onCompleted)
        {
            if (from == ShowHideState.None)
                ApplySettings(_hideSettings);

            Animate(_hideSettings, _showSettings, onCompleted);
        }

        protected override void OnHide(ShowHideState from, Action onCompleted)
        {
            if (from == ShowHideState.None)
                ApplySettings(_showSettings);

            Animate(_showSettings, _hideSettings, onCompleted);
        }

        protected override void OnSetShown(ShowHideState from)
            => ApplySettings(_showSettings);

        protected override void OnSetHidden(ShowHideState from)
            => ApplySettings(_hideSettings);

        protected override void OnStop(ShowHideState from)
            => _tween.Kill();

        private void Animate(DOTweenFloatSettings from, DOTweenFloatSettings to, Action onCompleted)
        {
            var range = to.Value - from.Value;
            var duration = range == 0f ? 0f : MathF.Abs((to.Value - Value) / range) * to.Duration;

            _tween.Kill();
            _tween = DOVirtual.Float(Value, to.Value, duration, _onUpdate)
                .SetUpdate(_isIndependentUpdate)
                .OnComplete(() => onCompleted());

            if (to.Ease.UseCurve)
                _tween.SetEase(to.Ease.Curve);
            else
                _tween.SetEase(to.Ease.Ease);
        }

        private void ApplySettings(DOTweenFloatSettings settings)
        {
            _tween.Kill();
            SetValue(settings.Value);
        }

        private void SetValue(float value)
        {
            Value = value;
            _setValue(value);
        }
    }
}
