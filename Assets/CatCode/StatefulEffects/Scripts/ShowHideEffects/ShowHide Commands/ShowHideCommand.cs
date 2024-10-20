using CatCode.Commands;


namespace CatCode.StatefulEffects.Commands
{
    public class ShowHideCommand
    {
        public static ShowHideCommand<T> Create<T>(T showHide, ShowHideCommandMode mode) where T : IShowHide
           => new(showHide, mode);

        public static ShowHideCommand<T> Show<T>(T showHide) where T : IShowHide
            => new(showHide, ShowHideCommandMode.Show);

        public static ShowHideCommand<T> Hide<T>(T showHide) where T : IShowHide
            => new(showHide, ShowHideCommandMode.Hide);

        public static ShowHideCommand<T> ShowAndWait<T>(T showHide) where T : IShowHide
        {
            return new ShowHideCommand<T>(showHide, ShowHideCommandMode.Show)
                .AddOnStarted(() => showHide.Show());
        }

        public static ShowHideCommand<T> HideAndWait<T>(T showHide) where T : IShowHide
        {
            return new ShowHideCommand<T>(showHide, ShowHideCommandMode.Hide)
                .AddOnStarted(() => showHide.Hide());
        }
    }

    public sealed class ShowHideCommand<T> : Command where T : IShowHide
    {
        private readonly T _showHideEffect;
        private readonly ShowHideCommandMode _mode;
        private readonly ShowHideState _targetState;

        public T ShowHideEffect => _showHideEffect;

        public ShowHideCommand(T showHideEffect, ShowHideCommandMode mode)
        {
            _showHideEffect = showHideEffect;
            _mode = mode;
            switch (_mode)
            {
                case ShowHideCommandMode.Show: _targetState = ShowHideState.Shown; break;
                case ShowHideCommandMode.Hide: _targetState = ShowHideState.Hidden; break;
            }
        }

        protected override void OnExecute()
        {
            var monoShowHide = _showHideEffect as MonoShowHide;
            if (monoShowHide == null || !monoShowHide)
            {
                Continue();
                return;
            }

            if (_showHideEffect.State == _targetState)
            {
                Continue();
                return;
            }

            _showHideEffect.StateChanged += OnShowHideEffectStateChanged;
            switch (_mode)
            {
                case ShowHideCommandMode.Show: _showHideEffect.Show(); break;
                case ShowHideCommandMode.Hide: _showHideEffect.Hide(); break;
                default: Continue(); break;
            }
        }


        protected override void OnStop()
        {
            _showHideEffect.StateChanged -= OnShowHideEffectStateChanged;
        }

        private void OnShowHideEffectStateChanged(ShowHideState state)
        {
            if (state != _targetState)
                return;
            _showHideEffect.StateChanged -= OnShowHideEffectStateChanged;
            Continue();
        }
    }
}