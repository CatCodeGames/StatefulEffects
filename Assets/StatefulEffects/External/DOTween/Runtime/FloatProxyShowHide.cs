#if STATEFULEFFECTS_DOTWEEN_SUPPORT

using CatCode.Events;
using DG.Tweening;
using System;


namespace CatCode.StatefulEffects
{
    public sealed class FloatProxyShowHide : IShowHide
    {
        private readonly IShowHide _stm;

        private Tween _tween;

        private readonly TweenCallback<float> _onUpdate;
        private readonly Func<float> _getValue;
        private readonly Action<float> _setValue;

        private readonly FloatEffectSettings _showSettings;
        private readonly FloatEffectSettings _hideSettings;

        public FloatProxyShowHide(
            ShowHideState initialState,
            bool ignoreState,
            Func<float> getValue,
            Action<float> setValue,
            FloatEffectSettings showSettings,
            FloatEffectSettings hideSettings)
        {
            _getValue = getValue;
            _setValue = setValue;

            _showSettings = showSettings;
            _hideSettings = hideSettings;

            _onUpdate = value => _setValue(value);

            _stm = new ShowHideCallbackStateMachine(
                initialState,
                ignoreState,
                OnShow,
                OnHide,
                OnSetShown,
                OnSetHidden,
                OnStop);
        }

        private void OnShow(Action onCompleted)
        {
            _tween.Kill();
            _tween = FloatTweenUtility.Play(_getValue(), _hideSettings, _showSettings, _onUpdate)
                .OnComplete(() => onCompleted());
        }

        private void OnSetShown()
            => ApplySettings(_showSettings);

        private void OnHide(Action onCompleted)
        {
            _tween.Kill();
            _tween = FloatTweenUtility.Play(_getValue(), _showSettings, _hideSettings, _onUpdate)
                .OnComplete(() => onCompleted());
        }

        private void OnSetHidden()
            => ApplySettings(_hideSettings);

        private void OnStop()
            => _tween.Kill();

        private void ApplySettings(FloatEffectSettings settings)
        {
            _tween.Kill();
            _setValue(settings.Value);
        }


        #region IShowHide

        public bool IsRunning => _stm.IsRunning;
        public IReadOnlyEventValue<ShowHideState> State => _stm.State;
        public void Hide() => _stm.Hide();
        public void SetHidden() => _stm.SetHidden();
        public void SetShown() => _stm.SetShown();
        public void Show() => _stm.Show();
        public void Stop() => _stm.Stop();

        #endregion
    }
}
#endif