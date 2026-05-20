#if STATEFULEFFECTS_UNITASK_SUPPORT

using CatCode.Events;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    public sealed class ShowHideUniTaskStateMachine : IShowHide
    {
        private readonly static Func<CancellationToken, UniTask> s_emptyAction = (t)
            => t.IsCancellationRequested
                ? UniTask.FromCanceled(t)
                : UniTask.CompletedTask;

        private readonly static Action s_emptySet = () => { };

        private bool _inProcess;
        private CancellationTokenSource _cts = new();

        private readonly EventValue<ShowHideState> _state;
        private readonly bool _ignoreState;
        private readonly Func<CancellationToken, UniTask> _onShow;
        private readonly Func<CancellationToken, UniTask> _onHide;
        private readonly Action _onSetShown;
        private readonly Action _onSetHidden;
        private readonly Action _onStop;

        public ShowHideUniTaskStateMachine(
            ShowHideState state,
            bool ignoreState,
            Func<CancellationToken, UniTask> onShow,
            Func<CancellationToken, UniTask> onHide,
            Action onShown,
            Action onHidden,
            Action onStop)
        {
            _state = new EventValue<ShowHideState>(state);
            _ignoreState = ignoreState;
            _onShow = onShow ?? s_emptyAction;
            _onHide = onHide ?? s_emptyAction;
            _onSetShown = onShown ?? s_emptySet;
            _onSetHidden = onHidden ?? s_emptySet;
            _onStop = onStop ?? s_emptySet;
        }

        public IReadOnlyEventValue<ShowHideState> State => _state;

        public void Show()
        {
            if (!_ignoreState && (_state.Value is ShowHideState.Showing or ShowHideState.Shown))
                return;

            RenewTokenSource();
            ProcessAsync(_onShow, ShowHideState.Showing, ShowHideState.Shown).Forget();
        }

        public void SetShown()
        {
            if (!_ignoreState && (_state.Value is ShowHideState.Shown))
                return;

            RenewTokenSource();
            _onSetShown();
        }

        public void Hide()
        {
            if (!_ignoreState && (_state.Value is ShowHideState.Hiding or ShowHideState.Hidden))
                return;

            RenewTokenSource();
            ProcessAsync(_onHide, ShowHideState.Hiding, ShowHideState.Hidden).Forget();
        }

        public void SetHidden()
        {
            if (!_ignoreState && (_state.Value is ShowHideState.Hidden))
                return;

            RenewTokenSource();
            _onSetHidden();
        }

        public void Stop()
        {
            RenewTokenSource();
            _onStop();
        }

        private async UniTaskVoid ProcessAsync(Func<CancellationToken, UniTask> taskFunc, ShowHideState fromState, ShowHideState toState)
        {
            try
            {
                _inProcess = true;
                var task = taskFunc(_cts.Token);
                _state.Value = fromState;
                await task;
                _inProcess = false;
                _state.Value = toState;
            }
            catch (OperationCanceledException) { }
        }

        private void RenewTokenSource()
        {
            if (!_inProcess)
                return;
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            _inProcess = false;
        }
    }
}
#endif