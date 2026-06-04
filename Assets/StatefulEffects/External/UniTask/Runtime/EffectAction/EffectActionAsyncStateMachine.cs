#if STATEFULEFFECTS_UNITASK_SUPPORT

using CatCode.Events;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    public sealed class EffectActionAsyncStateMachine : IEffectActionAsync
    {
        private bool _inTransition;
        private bool _isRunning;

        private CancellationTokenSource _cts;
        private AutoResetMultiUniTaskCompletionSource<StateTransitionResult> _tcs;

        private readonly EventValue<EffectActionState> _state;
        private readonly bool _ignoreState;
        private readonly Func<CancellationToken, UniTask> _onPlay;
        private readonly Action _onPrepare;
        private readonly Action _onStop;

        public bool IgnoreState => _ignoreState;
        public bool IsRunning => _isRunning;
        public IReadOnlyEventValue<EffectActionState> State => _state;


        public EffectActionAsyncStateMachine(
            EffectActionState initialState,
            bool ignoreState,
            Func<CancellationToken, UniTask> onPlay,
            Action onPrepare,
            Action onStop)
        {
            _onPlay = onPlay;
            _onPrepare = onPrepare;
            _onStop = onStop;

            _state = new EventValue<EffectActionState>(initialState);
            _ignoreState = ignoreState;
        }

        public UniTask<StateTransitionResult> PlayAsync()
        {
            if (_isRunning)
            {
                if (!_ignoreState)
                {
                    if (_inTransition && _state.Value == EffectActionState.Playing)
                        return _tcs.Task;
                }
                if (_inTransition)
                {
                    CancelTransition();
                    _tcs.TrySetResult(StateTransitionResult.Interrupted);
                }
            }
            _isRunning = true;
            RunTransitionAsync().Forget();
            return _tcs.Task;
        }

        public void Play() => PlayAsync().Forget();

        public void Prepare()
        {
            if (_isRunning && !_ignoreState && _state.Value == EffectActionState.Idle)
                return;
            _isRunning = true;
            if (_inTransition)
            {
                var tcs = _tcs;
                CancelTransition();

                _onPrepare();
                _state.Value = EffectActionState.Idle;

                tcs.TrySetResult(StateTransitionResult.ForceCompleted);
            }
            else
            {
                _onPrepare();
                _state.Value = EffectActionState.Idle;
            }
        }


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


        private async UniTaskVoid RunTransitionAsync()
        {
            _cts ??= new CancellationTokenSource();
            _tcs = AutoResetMultiUniTaskCompletionSource<StateTransitionResult>.Create();
            var tcs = _tcs;

            try
            {
                _inTransition = true;
                _state.Value = EffectActionState.Playing;
                await _onPlay(_cts.Token);
                _inTransition = false;
                _state.Value = EffectActionState.Idle;
                tcs.TrySetResult(StateTransitionResult.Completed);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void CancelTransition()
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;

            _inTransition = false;
        }
    }
}

#endif