#if STATEFULEFFECTS_UNITASK_SUPPORT

using CatCode.Events;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace CatCode.StatefulEffects
{

    public abstract class MonoShowHideAsync : MonoShowHide, IShowHideAsync
    {
        private IShowHideAsync _stm;

        [SerializeField] private ShowHideState _initialState = ShowHideState.Shown;
        [SerializeField] private bool _ignoreState = false;


        public override bool Initialized { get; protected set; }
        public override bool IsRunning => _stm.IsRunning;
        public override IReadOnlyEventValue<ShowHideState> State => _stm.State;

        private void Awake()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            _stm.Stop();
        }

        public override void Initialize()
        {
            if (Initialized)
                return;
            _stm = new ShowHideAsyncStateMachine(
                _initialState, _ignoreState,
                OnShow, OnHide,
                OnSetShown, OnSetHidden,
                OnStop);
            OnInitialize();

            Initialized = true;
        }

        public override void Show() => _stm.Show();
        public override void Hide() => _stm.Hide();

        public UniTask<StateTransitionResult> ShowAsync() => _stm.ShowAsync();
        public UniTask<StateTransitionResult> HideAsync() => _stm.HideAsync();

        public override void SetShown() => _stm.SetShown();
        public override void SetHidden() => _stm.SetHidden();

        public override void Stop() => _stm.Stop();


        protected virtual void OnInitialize() { }
        protected abstract UniTask OnShow(CancellationToken token);
        protected abstract UniTask OnHide(CancellationToken token);
        protected abstract void OnSetShown();
        protected abstract void OnSetHidden();
        protected abstract void OnStop();
    }
}
#endif