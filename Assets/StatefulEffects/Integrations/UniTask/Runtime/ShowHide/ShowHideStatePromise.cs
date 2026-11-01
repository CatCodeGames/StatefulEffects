using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Pool;

namespace CatCode.StatefulEffects
{
    internal sealed class ShowHideStatePromise : IUniTaskSource<ShowHideResult>
    {
        private static readonly ObjectPool<ShowHideStatePromise> s_pool = new(() => new());

        static ShowHideStatePromise()
        {
            TaskPool.RegisterSizeGetter(typeof(ShowHideStatePromise), () => s_pool.CountAll);
        }

        private readonly Action<ShowHideState> _stateChangedHandler;
        private readonly Action _cancellationHandler;

        private IShowHide _showHide;
        private ShowHideState _target;
        private ShowHideState _transition;
        private ShowHideWaitMode _mode;
        private CancellationToken _cancellationToken;
        private CancellationTokenRegistration _cancellationTokenRegistration;
        private UniTaskCompletionSourceCore<ShowHideResult> _core;

        private ShowHideStatePromise()
        {
            _stateChangedHandler = OnStateChanged;
            _cancellationHandler = OnCanceled;
        }

        public static UniTask<ShowHideResult> WaitAsync(
            IShowHide showHide,
            ShowHideState target,
            ShowHideState transition,
            ShowHideWaitMode mode,
            CancellationToken cancellationToken)
        {
            var promise = s_pool.Get();

            promise._core.Reset();
            promise._showHide = showHide;
            promise._target = target;
            promise._transition = transition;
            promise._mode = mode;
            promise._cancellationToken = cancellationToken;

            showHide.OnStateChanged += promise._stateChangedHandler;

            if (cancellationToken.CanBeCanceled)
                promise._cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(promise._cancellationHandler);

            TaskTracker.TrackActiveTask(promise, 3);

            return new UniTask<ShowHideResult>(promise, promise._core.Version);
        }

        private void OnStateChanged(ShowHideState state)
        {
            if (_core.UnsafeGetStatus() != UniTaskStatus.Pending)
                return;

            if (state == _target)
                Finish(ShowHideResult.Completed);
            else if (state == ShowHideState.None)
                Finish(ShowHideResult.Stopped);
            else if (_mode == ShowHideWaitMode.FirstChange && state != _transition)
                Finish(ShowHideResult.Interrupted);
        }

        private void Finish(ShowHideResult result)
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
            _showHide.OnStateChanged -= _stateChangedHandler;
            _cancellationTokenRegistration.Dispose();
        }

        public ShowHideResult GetResult(short token)
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
            _showHide = null;
            _target = default;
            _transition = default;
            _mode = default;
            _cancellationToken = default;
            _cancellationTokenRegistration = default;

            s_pool.Release(this);
        }
    }
}
