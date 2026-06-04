#if STATEFULEFFECTS_UNITASK_SUPPORT

using CatCode.Events;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    public sealed class ShowHideAsyncStateMachine : IShowHideAsync
    {
        private readonly static Func<CancellationToken, UniTask> s_emptyAction = (t)
            => t.IsCancellationRequested ? UniTask.FromCanceled(t) : UniTask.CompletedTask;

        private readonly static Action s_emptySet = () => { };

        private bool _isRunning = false;
        private bool _inTransition = false;
        private CancellationTokenSource _cts;
        private AutoResetMultiUniTaskCompletionSource<StateTransitionResult> _tcs;

        private readonly EventValue<ShowHideState> _state;
        private readonly bool _ignoreState;
        private readonly Func<CancellationToken, UniTask> _onShow;
        private readonly Func<CancellationToken, UniTask> _onHide;
        private readonly Action _onSetShown;
        private readonly Action _onSetHidden;
        private readonly Action _onStop;

        public bool IsRunning => _isRunning;
        public IReadOnlyEventValue<ShowHideState> State => _state;


        public ShowHideAsyncStateMachine(
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

        public void Show()
            => ShowAsync().Forget();

        public void Hide()
            => HideAsync().Forget();


        public UniTask<StateTransitionResult> ShowAsync()
            => RunAsync(_onShow, ShowHideState.Showing, ShowHideState.Shown);

        public UniTask<StateTransitionResult> HideAsync()
             => RunAsync(_onHide, ShowHideState.Hiding, ShowHideState.Hidden);


        public void SetShown()
            => SetState(_onSetShown, ShowHideState.Shown, ShowHideState.Shown);

        public void SetHidden()
            => SetState(_onSetHidden, ShowHideState.Hiding, ShowHideState.Hidden);


        public void Stop()
        {
            if (_inTransition)
            {
                var tcs = _tcs;
                CancelTransition();

                _onStop();
                _isRunning = false;

                tcs.TrySetResult(StateTransitionResult.Interrupted);
            }
            else
            {
                _onStop();
                _isRunning = false;
            }
        }


        private void CancelTransition()
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;

            _inTransition = false;
        }


        private UniTask<StateTransitionResult> RunAsync(
            Func<CancellationToken, UniTask> taskFunc,
            ShowHideState transitionState,
            ShowHideState targetState)
        {
            if (_isRunning)
            {
                if (!_ignoreState)
                {
                    if (_inTransition && _state.Value == transitionState)
                        return _tcs.Task;
                    if (_state.Value == targetState)
                        return UniTask.FromResult(StateTransitionResult.AlreadyInState);
                }

                if (_inTransition)
                {
                    CancelTransition();
                    _tcs.TrySetResult(StateTransitionResult.Interrupted);
                }
            }
            _isRunning = true;
            RunTransitionAsync(taskFunc, transitionState, targetState).Forget();
            return _tcs.Task;
        }

        private async UniTaskVoid RunTransitionAsync(
            Func<CancellationToken, UniTask> taskFunc,
            ShowHideState fromState,
            ShowHideState toState)
        {
            _cts ??= new CancellationTokenSource();
            _tcs = AutoResetMultiUniTaskCompletionSource<StateTransitionResult>.Create();
            var tcs = _tcs;

            try
            {
                _inTransition = true;
                _state.Value = fromState;
                await taskFunc(_cts.Token);
                _inTransition = false;
                _state.Value = toState;
                tcs.TrySetResult(StateTransitionResult.Completed);                
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void SetState(
            Action onSet,
            ShowHideState transitionState,
            ShowHideState targetState)
        {
            if (_isRunning && !_ignoreState && _state.Value == targetState)
                return;
            _isRunning = true;
            if (_inTransition)
            {
                var transitionResult = _state.Value == transitionState
                    ? StateTransitionResult.ForceCompleted
                    : StateTransitionResult.Interrupted;

                var tcs = _tcs;
                CancelTransition();

                onSet();
                _state.Value = targetState;

                tcs.TrySetResult(transitionResult);
            }
            else
            {
                onSet();
                _state.Value = targetState;
            }
        }
    }
}
#endif