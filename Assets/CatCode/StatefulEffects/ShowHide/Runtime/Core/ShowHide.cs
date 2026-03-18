using CatCode.Common;
using System;

namespace CatCode.StatefulEffects
{
    public abstract class ShowHide : IShowHide
    {
        private readonly StateTransition<ShowHideState> _transition;
        private readonly bool _allowInterrupt;

        public IReadOnlyEventValue<ShowHideState> State => _transition.State;

        public ShowHide(ShowHideState initialState, bool allowInterrupt = false)
        {
            _allowInterrupt = allowInterrupt;
            _transition = new StateTransition<ShowHideState>(initialState);
        }


        public void Show()
            => _transition.Run(ShowHideState.Show, ShowHideState.Shown, OnShow, _allowInterrupt);

        public void SetShown()
            => _transition.Set(ShowHideState.Shown, OnSetShown);

        public void Hide()
            => _transition.Run(ShowHideState.Hide, ShowHideState.Hidden, OnHide, _allowInterrupt);

        public void SetHidden()
            => _transition.Set(ShowHideState.Hidden, OnSetHidden);

        public void Stop()
        {
            OnStop();
            _transition.Stop();
        }

        protected abstract void OnShow(Action onCompleted);
        protected abstract void OnHide(Action onCompleted);
        protected abstract void OnSetShown();
        protected abstract void OnSetHidden();
        protected abstract void OnStop();
    }
}
