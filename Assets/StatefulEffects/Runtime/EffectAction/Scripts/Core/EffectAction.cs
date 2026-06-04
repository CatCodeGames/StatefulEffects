using CatCode.Events;
using System;

namespace CatCode.StatefulEffects
{
    public abstract class EffectAction : IEffectAction
    {
        private readonly IEffectAction _stm;

        public bool IsRunning => _stm.IsRunning;
        public IReadOnlyEventValue<EffectActionState> State => _stm.State;


        protected EffectAction(EffectActionState initialState, bool ignoreState)
        {
            _stm = new EffectActionCallbackStateMachine(
                initialState,
                ignoreState,
                OnPlay,
                OnPrepare,
                OnStop);
        }


        public void Play() => _stm.Play();
        public void Prepare() => _stm.Prepare();
        public void Stop() => _stm.Stop();


        protected abstract void OnPlay(Action onCompleted);
        protected abstract void OnPrepare();
        protected abstract void OnStop();
    }
}
