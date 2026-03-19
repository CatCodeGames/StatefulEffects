using CatCode.Common;
using System;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    public abstract class MonoShowHideTemplate : MonoShowHide
    {
        private bool _initialized;
        private StateTransition<ShowHideState> _transition;

        [SerializeField] private bool _allowInterrupt;
        [SerializeField] private ShowHideState _initialState;

        public override IReadOnlyEventValue<ShowHideState> State => _transition.State;

        protected virtual void Awake()
            => Initialize();

        public override void Initialize()
        {
            if (_initialized)
                return;
            _initialized = true;
            _transition = new StateTransition<ShowHideState>(_initialState);
        }

        public override void Hide()
            => _transition.Run(ShowHideState.Hiding, ShowHideState.Hidden, OnHide, _allowInterrupt);

        public override void SetHidden()
            => _transition.Set(ShowHideState.Hidden, OnSetHidden);

        public override void SetShown()
            => _transition.Set(ShowHideState.Shown, OnSetShown);

        public override void Show()
            => _transition.Run(ShowHideState.Showing, ShowHideState.Shown, OnShow, _allowInterrupt);

        public override void Stop()
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
