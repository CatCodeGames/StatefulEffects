using CatCode.Common;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine.Pool;

namespace CatCode.Promises
{
    public sealed class EventRequestPromise : IUniTaskSource
    {
        private readonly static ObjectPool<EventRequestPromise> s_pool = new(() => new());

        static EventRequestPromise()
        {
            TaskPool.RegisterSizeGetter(typeof(EventRequestPromise), () => s_pool.CountAll);
        }

        private readonly Action _handler;
        private readonly Action _cancellationAction;

        private IReadOnlyEventRequest _eventRequest;
        private CancellationToken _cancellationToken;
        private CancellationTokenRegistration _cancellationTokenRegistration;
        private UniTaskCompletionSourceCore<AsyncUnit> _core;

        public short Version
            => _core.Version;

        public EventRequestPromise()
        {
            _handler = EventHandler;
            _cancellationAction = OnCancel;
        }

        private void Init(IReadOnlyEventRequest eventRequest, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _eventRequest = eventRequest;

            _eventRequest.Requested += _handler;

            _core.Reset();
            if (_cancellationToken.CanBeCanceled)
            {
                _cancellationTokenRegistration = _cancellationToken.RegisterWithoutCaptureExecutionContext(_cancellationAction);
            }
        }

        public static IUniTaskSource Create(IReadOnlyEventRequest eventRequest, CancellationToken cancellationToken, out short token)
        {
            if (cancellationToken.IsCancellationRequested)
                return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
            if (eventRequest.IsRequested)
                return AutoResetUniTaskCompletionSource.CreateCompleted(out token);

            var promise = s_pool.Get();
            promise.Init(eventRequest, cancellationToken);

            TaskTracker.TrackActiveTask(promise, 3);

            token = promise.Version;
            return promise;
        }

        private void EventHandler()
        {
            _eventRequest.Requested -= _handler;
            _cancellationTokenRegistration.Dispose();
            _core.TrySetResult(AsyncUnit.Default);
        }

        private void OnCancel()
        {
            _eventRequest.Requested -= _handler;
            _cancellationTokenRegistration.Dispose();

            _core.TrySetCanceled(_cancellationToken);
        }

        public void GetResult(short token)
        {
            try
            {
                _core.GetResult(token);
            }
            finally
            {
                Release();
            }
        }

        public UniTaskStatus GetStatus(short token)
            => _core.GetStatus(token);

        public UniTaskStatus UnsafeGetStatus()
            => _core.UnsafeGetStatus();

        public void OnCompleted(Action<object> continuation, object state, short token)
            => _core.OnCompleted(continuation, state, token);

        private void Release()
        {
            TaskTracker.RemoveTracking(this);

            _eventRequest = null;
            _cancellationToken = default;

            s_pool.Release(this);
        }
    }
}