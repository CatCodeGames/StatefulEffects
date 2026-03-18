using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace CatCode.Common
{
    public sealed class CompositeDisposable : IDisposable
    {
        private List<IDisposable> _disposable;

        public CompositeDisposable()
        {
            _disposable = ListPool<IDisposable>.Get();
        }

        public bool IsReleased => _disposable == null;

        public CompositeDisposable Add(IDisposable disposable)
        {
            _disposable.Add(disposable);
            return this;
        }

        public void Dispose()
        {
            foreach (var diposable in _disposable)
                diposable.Dispose();
            ListPool<IDisposable>.Release(_disposable);
            _disposable = null;
        }
    }
}