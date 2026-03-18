using CatCode.Common;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine.Pool;

namespace CatCode.Promises
{
    public sealed class EventValuePromise<T> : IUniTaskSource
    {
        private readonly static ObjectPool<EventValuePromise<T>> s_pool = new(() => new());

        static EventValuePromise()
        {
            TaskPool.RegisterSizeGetter(typeof(EventValuePromise<T>), () => s_pool.CountAll);
        }

        private readonly Action<T> _handler;
        private readonly Action _cancellationAction;

        private IReadOnlyEventValue<T> _eventValue;
        private ICondition _predicate;
        private CancellationToken _cancellationToken;
        private CancellationTokenRegistration _cancellationTokenRegistration;
        private UniTaskCompletionSourceCore<AsyncUnit> _core;

        public short Version
            => _core.Version;

        public EventValuePromise()
        {
            _handler = EventHandler;
            _cancellationAction = OnCancel;
        }

        private void Init(IReadOnlyEventValue<T> eventValue, ICondition predicate, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _eventValue = eventValue;
            _predicate = predicate;
          
            _eventValue.Changed += _handler;

            _core.Reset();
            if (_cancellationToken.CanBeCanceled)
            {
                _cancellationTokenRegistration = _cancellationToken.RegisterWithoutCaptureExecutionContext(_cancellationAction);
            }
        }


        public static IUniTaskSource Create(IReadOnlyEventValue<T> eventValue, ICondition predicate, CancellationToken cancellationToken, out short token)
        {
            if (cancellationToken.IsCancellationRequested)            
                return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
            if (predicate.Check())
                return AutoResetUniTaskCompletionSource.CreateCompleted(out  token);

            var promise = s_pool.Get();
            promise.Init(eventValue, predicate, cancellationToken);

            TaskTracker.TrackActiveTask(promise, 3);

            token = promise.Version;
            return promise;
        }


        private void EventHandler(T value)
        {
            if (!_predicate.Check())
                return;

            _eventValue.Changed -= _handler;
            _cancellationTokenRegistration.Dispose();
            _core.TrySetResult(AsyncUnit.Default);
        }

        private void OnCancel()
        {
            _eventValue.Changed -= _handler;
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

            _eventValue = null;
            _predicate = null;
            _cancellationToken = default;

            s_pool.Release(this);
        }
    }
}