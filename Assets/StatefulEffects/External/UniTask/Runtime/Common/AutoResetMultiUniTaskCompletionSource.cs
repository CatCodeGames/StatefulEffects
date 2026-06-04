#if STATEFULEFFECTS_UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.Pool;

namespace CatCode.StatefulEffects
{
    public sealed class AutoResetMultiUniTaskCompletionSource<T> : IPromise<T>
    {
        private readonly List<AutoResetUniTaskCompletionSource<T>> _tcs = new();
        private static readonly ObjectPool<AutoResetMultiUniTaskCompletionSource<T>> s_pool;

        static AutoResetMultiUniTaskCompletionSource()
        {
            s_pool = new ObjectPool<AutoResetMultiUniTaskCompletionSource<T>>(createFunc: () => new());
        }

        public static AutoResetMultiUniTaskCompletionSource<T> Create()
        {
            var instance =  s_pool.Get();
            return instance;
        }

        public UniTask<T> Task
        {
            get
            {
                var tcs = AutoResetUniTaskCompletionSource<T>.Create();
                _tcs.Add(tcs);
                return tcs.Task;
            }
        }

        public bool TrySetCanceled(CancellationToken cancellationToken = default)
        {
            if (_tcs.Count == 0)
                return false;
            for (int i = 0; i < _tcs.Count; i++)
                _tcs[i].TrySetCanceled(cancellationToken);
            _tcs.Clear();
            s_pool.Release(this);
            return true;
        }

        public bool TrySetException(Exception exception)
        {
            if (_tcs.Count == 0)
                return false;
            for (int i = 0; i < _tcs.Count; i++)
                _tcs[i].TrySetException(exception);
            _tcs.Clear();
            s_pool.Release(this);
            return true;
        }

        public bool TrySetResult(T result)
        {
            if (_tcs.Count == 0)
                return false;
            for (int i = 0; i < _tcs.Count; i++)
                _tcs[i].TrySetResult(result);
            _tcs.Clear();
            s_pool.Release(this);
            return true;
        }
    }
}
#endif