using CatCode.Events;
using System;

namespace CatCode.StatefulEffects
{
    public sealed class ShowHideCallbackStateMachine : IShowHide
    {
        private readonly static Action<Action> s_emptyAction = (callback) => callback();
        private readonly static Action s_emptySet = () => { };

        private readonly EventValue<ShowHideState> _state;
        private readonly bool _ignoreState;

        private readonly Action<Action> _onShow;
        private readonly Action<Action> _onHide;
        private readonly Action _onSetShown;
        private readonly Action _onSetHidden;
        private readonly Action _onStop;

        private readonly Action _onShowCompleted;
        private readonly Action _onHideCompleted;

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

            _onShow = onShow ?? s_emptyAction;
            _onHide = onHide ?? s_emptyAction;
            _onSetShown = onSetShown ?? s_emptySet;
            _onSetHidden = onSetHidden ?? s_emptySet;
            _onStop = onStop ?? s_emptySet;

            _onShowCompleted = OnShowCompleted;
            _onHideCompleted = OnHideCompleted;
        }

        public void Show()
        {
            if (!_ignoreState && (_state.Value is ShowHideState.Showing or ShowHideState.Shown))
                return;
            _state.Value = ShowHideState.Shown;
            _onShow(_onShowCompleted);
        }

        public void SetShown()
        {
            if (!_ignoreState && (_state.Value is ShowHideState.Shown))
                return;
            _onSetShown();
            _state.Value = ShowHideState.Shown;
        }

        public void Hide()
        {
            if (!_ignoreState && (_state.Value is ShowHideState.Hiding or ShowHideState.Hidden))
                return;

            _state.Value = ShowHideState.Hiding;
            _onHide(_onHideCompleted);
        }

        public void SetHidden()
        {
            if (!_ignoreState && (_state.Value is ShowHideState.Hidden))
                return;

            _onSetHidden();
            _state.Value = ShowHideState.Hidden;
        }

        public void Stop()
        {
            _onStop();
        }

        private void OnShowCompleted() => _state.Value = ShowHideState.Shown;
        private void OnHideCompleted() => _state.Value = ShowHideState.Hidden;
    }
}