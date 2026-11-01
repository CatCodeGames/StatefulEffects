using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Pool;

namespace CatCode.StatefulEffects
{
    internal sealed class EffectActionStatePromise : IUniTaskSource<EffectActionResult>
    {
        private static readonly ObjectPool<EffectActionStatePromise> s_pool = new(() => new());

        static EffectActionStatePromise()
        {
            TaskPool.RegisterSizeGetter(typeof(EffectActionStatePromise), () => s_pool.CountAll);
        }

        private readonly Action<EffectActionState> _stateChangedHandler;
        private readonly Action _cancellationHandler;

        private IEffectAction _effectAction;
        private EffectActionState _target;
        private CancellationToken _cancellationToken;
        private CancellationTokenRegistration _cancellationTokenRegistration;
        private UniTaskCompletionSourceCore<EffectActionResult> _core;

        private EffectActionStatePromise()
        {
            _stateChangedHandler = OnStateChanged;
            _cancellationHandler = OnCanceled;
        }

        public static UniTask<EffectActionResult> WaitAsync(
            IEffectAction effectAction,
            EffectActionState target,
            CancellationToken cancellationToken)
        {
            var promise = s_pool.Get();

            promise._core.Reset();
            promise._effectAction = effectAction;
            promise._target = target;
            promise._cancellationToken = cancellationToken;

            effectAction.OnStateChanged += promise._stateChangedHandler;

            if (cancellationToken.CanBeCanceled)
                promise._cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(promise._cancellationHandler);

            TaskTracker.TrackActiveTask(promise, 3);

            return new UniTask<EffectActionResult>(promise, promise._core.Version);
        }

        private void OnStateChanged(EffectActionState state)
        {
            if (_core.UnsafeGetStatus() != UniTaskStatus.Pending)
                return;

            if (state == _target)
                Finish(EffectActionResult.Completed);
            else if (state == EffectActionState.None)
                Finish(EffectActionResult.Stopped);
        }

        private void Finish(EffectActionResult result)
        {
            StopListening();
            _core.TrySetResult(result);
        }

        private void OnCanceled()
        {
            if (_core.UnsafeGetStatus() != UniTaskStatus.Pending)
                return;

            StopListening();
            _core.TrySetCanceled(_cancellationToken);
        }

        private void StopListening()
        {
            _effectAction.OnStateChanged -= _stateChangedHandler;
            _cancellationTokenRegistration.Dispose();
        }

        public EffectActionResult GetResult(short token)
        {
            try
            {
                return _core.GetResult(token);
            }
            finally
            {
                TaskTracker.RemoveTracking(this);
                ReturnToPool();
            }
        }

        void IUniTaskSource.GetResult(short token)
            => GetResult(token);

        public UniTaskStatus GetStatus(short token)
            => _core.GetStatus(token);

        public UniTaskStatus UnsafeGetStatus()
            => _core.UnsafeGetStatus();

        public void OnCompleted(Action<object> continuation, object state, short token)
            => _core.OnCompleted(continuation, state, token);

        private void ReturnToPool()
        {
            _effectAction = null;
            _target = default;
            _cancellationToken = default;
            _cancellationTokenRegistration = default;

            s_pool.Release(this);
        }
    }
}
