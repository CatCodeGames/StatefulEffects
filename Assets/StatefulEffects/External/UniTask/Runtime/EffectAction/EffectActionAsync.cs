#if STATEFULEFFECTS_UNITASK_SUPPORT

using CatCode.Events;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace CatCode.StatefulEffects
{
    public abstract class EffectActionAsync : IEffectActionAsync
    {
        private readonly IEffectActionAsync _stm;

        public bool IsRunning => _stm.IsRunning;
        public IReadOnlyEventValue<EffectActionState> State => _stm.State;


        public EffectActionAsync(EffectActionState initialState, bool ignoreState)
        {
            _stm = new EffectActionAsyncStateMachine(
                initialState,
                ignoreState,
                OnPlay,
                OnPrepare,
                OnStop);
        }


        public UniTask<StateTransitionResult> PlayAsync() => _stm.PlayAsync();
        public void Play() => _stm.Play();
        public void Prepare() => _stm.Prepare();
        public void Stop() => _stm.Stop();


        protected abstract UniTask OnPlay(CancellationToken token);
        protected abstract void OnPrepare();
        protected abstract void OnStop();
    }
}

#endif