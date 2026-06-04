using CatCode.Events;
using System;

namespace CatCode.StatefulEffects
{
    public sealed class ShowHideCallbackStateMachine : IShowHide
    {
        private readonly static Action<Action> s_emptyCallbackAction = (callback) => callback();
        private readonly static Action s_emptyAction = () => { };

        private readonly EventValue<ShowHideState> _state;
        private readonly bool _ignoreState;

        private readonly Action<Action> _onShow;
        private readonly Action<Action> _onHide;
        private readonly Action _onSetShown;
        private readonly Action _onSetHidden;
        private readonly Action _onStop;

        private readonly Action _onShowCompleted;
        private readonly Action _onHideCompleted;

        private bool _isRunning;

        public bool IsRunning => _isRunning;
        public IReadOnlyEventValue<ShowHideState> State => _state;

        public ShowHideCallbackStateMachine(
            ShowHideState initialState,
            bool ignoreState,
            Action<Action> onShow,
            Action<Action> onHide,
            Action onSetShown,
            Action onSetHidden,
            Action onStop)
        {
            _state = new EventValue<ShowHideState>(initialState);
            _ignoreState = ignoreState;

            _onShow = onShow ?? s_emptyCallbackAction;
            _onHide = onHide ?? s_emptyCallbackAction;
            _onSetShown = onSetShown ?? s_emptyAction;
            _onSetHidden = onSetHidden ?? s_emptyAction;
            _onStop = onStop ?? s_emptyAction;

            _onShowCompleted = OnShowCompleted;
            _onHideCompleted = OnHideCompleted;
        }

        public void Show()
        {
            if (_isRunning && !_ignoreState && (_state.Value is ShowHideState.Showing or ShowHideState.Shown))
                return;
            _isRunning = true;
            _state.Value = ShowHideState.Showing;
            _onShow(_onShowCompleted);
        }

        public void SetShown()
        {
            if (_isRunning && !_ignoreState && (_state.Value is ShowHideState.Shown))
                return;
            _isRunning = true;
            _onSetShown();
            _state.Value = ShowHideState.Shown;
        }

        public void Hide()
        {
            if (_isRunning && !_ignoreState && (_state.Value is ShowHideState.Hiding or ShowHideState.Hidden))
                return;
            _isRunning = true;
            _state.Value = ShowHideState.Hiding;
            _onHide(_onHideCompleted);
        }

        public void SetHidden()
        {
            if (_isRunning && !_ignoreState && (_state.Value is ShowHideState.Hidden))
                return;
            _isRunning = true;
            _onSetHidden();
            _state.Value = ShowHideState.Hidden;
        }

        public void Stop()
        {
            _onStop();
            _isRunning = false;
        }

        private void OnShowCompleted() => _state.Value = ShowHideState.Shown;
        private void OnHideCompleted() => _state.Value = ShowHideState.Hidden;
    }
}