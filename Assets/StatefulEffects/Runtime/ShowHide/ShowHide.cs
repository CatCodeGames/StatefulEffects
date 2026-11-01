using System;

namespace CatCode.StatefulEffects
{
    public abstract class ShowHide : IShowHide
    {
        private ShowHideState _state;
        private int _currentTransition;

        private readonly Action<ShowHideState, Action> _onShow;
        private readonly Action<ShowHideState, Action> _onHide;
        private readonly Action<ShowHideState> _onSetShown;
        private readonly Action<ShowHideState> _onSetHidden;

        public ShowHideState State => _state;
        public event Action<ShowHideState> OnStateChanged;

        protected abstract ShowHidePolicy Policy { get; }

        protected ShowHide()
        {
            _onShow = OnShow;
            _onHide = OnHide;
            _onSetShown = OnSetShown;
            _onSetHidden = OnSetHidden;
        }

        public void Show()
        {
            switch (_state)
            {
                case ShowHideState.None:
                    {
                        Animate(ShowHideState.None, ShowHideState.Showing, ShowHideState.Shown, _onShow);
                        return;
                    }

                case ShowHideState.Showing:
                    {
                        return;
                    }

                case ShowHideState.Shown:
                    {
                        if (Policy.Show.OnRepeat == RepeatResolution.Ignore)
                            return;
                        OnSetHidden(ShowHideState.Shown);
                        Animate(ShowHideState.Hidden, ShowHideState.Showing, ShowHideState.Shown, _onShow);
                        return;
                    }

                case ShowHideState.Hiding:
                    {
                        if (Policy.Hide.OnInterrupt == InterruptResolution.Ignore)
                            return;
                        Animate(ShowHideState.Hiding, ShowHideState.Showing, ShowHideState.Shown, _onShow);
                        return;
                    }

                case ShowHideState.Hidden:
                    {
                        Animate(ShowHideState.Hidden, ShowHideState.Showing, ShowHideState.Shown, _onShow);
                        return;
                    }
            }

            throw new InvalidOperationException($"Unexpected state {_state}.");
        }

        public void Hide()
        {
            switch (_state)
            {
                case ShowHideState.None:
                    {
                        Animate(ShowHideState.None, ShowHideState.Hiding, ShowHideState.Hidden, _onHide);
                        return;
                    }

                case ShowHideState.Showing:
                    {
                        if (Policy.Show.OnInterrupt == InterruptResolution.Ignore)
                            return;
                        Animate(ShowHideState.Showing, ShowHideState.Hiding, ShowHideState.Hidden, _onHide);
                        return;
                    }

                case ShowHideState.Shown:
                    {
                        Animate(ShowHideState.Shown, ShowHideState.Hiding, ShowHideState.Hidden, _onHide);
                        return;
                    }

                case ShowHideState.Hiding:
                    {
                        return;
                    }

                case ShowHideState.Hidden:
                    {
                        if (Policy.Hide.OnRepeat == RepeatResolution.Ignore)
                            return;
                        OnSetShown(ShowHideState.Hidden);
                        Animate(ShowHideState.Shown, ShowHideState.Hiding, ShowHideState.Hidden, _onHide);
                        return;
                    }
            }

            throw new InvalidOperationException($"Unexpected state {_state}.");
        }

        public void SetShown()
        {
            if (_state == ShowHideState.Shown)
                return;

            Snap(ShowHideState.Shown, _onSetShown);
        }

        public void SetHidden()
        {
            if (_state == ShowHideState.Hidden)
                return;

            Snap(ShowHideState.Hidden, _onSetHidden);
        }

        public void Stop()
        {
            _currentTransition++;
            OnStop(_state);
            SetState(ShowHideState.None);
        }

        protected abstract void OnShow(ShowHideState from, Action onCompleted);
        protected abstract void OnHide(ShowHideState from, Action onCompleted);
        protected abstract void OnSetShown(ShowHideState from);
        protected abstract void OnSetHidden(ShowHideState from);
        protected abstract void OnStop(ShowHideState from);


        private void Animate(ShowHideState fromState, ShowHideState transitionState, ShowHideState endState, Action<ShowHideState, Action> play)
        {
            var transition = ++_currentTransition;
            SetState(transitionState);

            if (transition != _currentTransition)
                return;

            play(fromState, () =>
            {
                if (transition == _currentTransition)
                    SetState(endState);
            });
        }

        private void Snap(ShowHideState endState, Action<ShowHideState> apply)
        {
            var transition = ++_currentTransition;
            apply(_state);

            if (transition == _currentTransition)
                SetState(endState);
        }

        private void SetState(ShowHideState state)
        {
            if (_state == state)
                return;

            _state = state;
            OnStateChanged?.Invoke(_state);
        }
    }
}
