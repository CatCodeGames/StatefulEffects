using CatCode.Events;
using System;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    public abstract class CallbackMonoShowHide : MonoShowHide
    {
        private IShowHide _stm;

        [SerializeField] private ShowHideState _initialState;
        [SerializeField] private bool _ignoreState;


        private void Awake()
        {
            Initialize();
        }

        protected virtual void OnDestroy()
        {
            Stop();
        }

        public override void Initialize()
        {
            if (Initialized)
                return;

            _stm = new ShowHideCallbackStateMachine(
                _initialState,
                _ignoreState,
                OnShow,
                OnHide,
                OnSetShown,
                OnSetHidden,
                OnStop);

            OnInitialize();
            Initialized = true;
        }


        #region IShowHide 

        public override bool IsRunning => _stm.IsRunning;
        public override bool Initialized { get; protected set; }
        public override IReadOnlyEventValue<ShowHideState> State => _stm.State;
        public override void Show() => _stm.Show();
        public override void Hide() => _stm.Hide();
        public override void SetShown() => _stm.SetShown();
        public override void SetHidden() => _stm.SetHidden();
        public override void Stop() => _stm.Stop();

        #endregion

        protected virtual void OnInitialize() { }
        protected abstract void OnShow(Action onCompleted);
        protected abstract void OnHide(Action onCompleted);
        protected abstract void OnSetShown();
        protected abstract void OnSetHidden();
        protected abstract void OnStop();
    }
}
