using CatCode.Events;
using System;
using UnityEngine;

namespace CatCode.StatefulEffects
{

    public abstract class CallbackEffectAction : MonoEffectAction
    {
        private IEffectAction _stm;

        [SerializeField] private EffectActionState _initialState;
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

            _stm = new EffectActionCallbackStateMachine(
                _initialState,
                _ignoreState,
                OnPlay,
                OnPrepare,
                OnStop);

            OnInitialize();
            Initialized = true;
        }

        #region IEffectAction
        public override bool IsRunning => _stm.IsRunning;
        public override bool Initialized { get; protected set; }
        public override IReadOnlyEventValue<EffectActionState> State => _stm.State;
        public override void Play() => _stm.Play();
        public override void Prepare() => _stm.Prepare();
        public override void Stop() => _stm.Stop();

        #endregion

        protected virtual void OnInitialize() { }
        protected abstract void OnPlay(Action onCompleted);
        protected abstract void OnPrepare();
        protected abstract void OnStop();
    }
}
