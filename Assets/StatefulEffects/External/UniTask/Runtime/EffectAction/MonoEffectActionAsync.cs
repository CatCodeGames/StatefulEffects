#if STATEFULEFFECTS_UNITASK_SUPPORT

using CatCode.Events;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    public abstract class MonoEffectActionAsync : MonoBehaviour, IEffectActionAsync
    {
        private IEffectActionAsync _stm;

        [SerializeField] private EffectActionState _initialState;
        [SerializeField] private bool _ignoreState;


        private void Awake()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            Stop();
        }

        public void Initialize()
        {
            if (Initialized)
                return;

            _stm = new EffectActionAsyncStateMachine(
                _initialState,
                _ignoreState,
                OnPlay,
                OnPrepare,
                OnStop);

            OnInitialize();
            Initialized = true;
        }

        #region IEffectAction

        public bool Initialized { get; private set; }
        public bool IsRunning { get; private set; }
        public IReadOnlyEventValue<EffectActionState> State => _stm.State;
        public UniTask<StateTransitionResult> PlayAsync() => _stm.PlayAsync();
        public void Play() => _stm.Play();
        public void Prepare() => _stm.Prepare();
        public void Stop() => _stm.Stop();

        #endregion

        protected virtual void OnInitialize() { }
        protected abstract UniTask OnPlay(CancellationToken token);
        protected abstract void OnPrepare();
        protected abstract void OnStop();
    }
}

#endif