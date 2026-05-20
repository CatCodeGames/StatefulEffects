#if STATEFULEFFECTS_DOTWEEN_SUPPORT

using CatCode.Events;
using DG.Tweening;
using System;

namespace CatCode.StatefulEffects
{
    public sealed class FloatValueShowHide : IShowHide
    {
        private readonly IShowHide _showHide;

        private Tween _tween;
        private readonly TweenCallback<float> _onUpdate;

        private readonly FloatEffectSettings _showSettings;
        private readonly FloatEffectSettings _hideSettings;
        private readonly EventValue<float> _value;

        public FloatValueShowHide(
            ShowHideState initialState,
            float initialValue,
            bool ignoreState,
            FloatEffectSettings showSettings,
            FloatEffectSettings hideSettings)
        {
            _showSettings = showSettings;
            _hideSettings = hideSettings;
            _value = new EventValue<float>(initialValue, NotifyMode.Always);

            _showHide = new ShowHideCallbackStateMachine(
                initialState,
                ignoreState,
                OnShow,
                OnHide,
                OnSetShown,
                OnSetHidden,
                OnStop);

            _onUpdate = value => _value.Value = value;
        }

        private void OnShow(Action onCompleted)
        {
            _tween.Kill();
            _tween = FloatTweenUtility.Play(_value.Value, _hideSettings, _showSettings, _onUpdate)
                .OnComplete(() => onCompleted());
        }

        private void OnSetShown()
            => ApplySettings(_showSettings);

        private void OnHide(Action onCompleted)
        {
            _tween.Kill();
            _tween = FloatTweenUtility.Play(_value.Value, _showSettings, _hideSettings, _onUpdate)
                .OnComplete(() => onCompleted());
        }

        private void OnSetHidden()
            => ApplySettings(_hideSettings);

        private void OnStop()
            => _tween.Kill();

        private void ApplySettings(FloatEffectSettings settings)
        {
            _tween.Kill();
            _value.Value = settings.Value;
        }



        #region IShowHide

        public IReadOnlyEventValue<ShowHideState> State => _showHide.State;
        public void Hide() => _showHide.Hide();
        public void SetHidden() => _showHide.SetHidden();
        public void SetShown() => _showHide.SetShown();
        public void Show() => _showHide.Show();
        public void Stop() => _showHide.Stop();

        #endregion
    }
}
#endif