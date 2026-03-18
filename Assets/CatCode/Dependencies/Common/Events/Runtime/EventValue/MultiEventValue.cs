using System;
using System.Collections.Generic;

namespace CatCode.Common
{
    public sealed class MultiEventValue<T> : IReadOnlyEventValue<T>, IDisposable
    {
        private readonly IReadOnlyEventValue<T>[] _sources;
        private readonly Func<T[], T> _combine;
        private readonly EqualityComparer<T> _comparer;

        private readonly T[] _buffer;
        private T _value;

        public T Value => _value;

        public event Action<T> Changed;

        public MultiEventValue(Func<T[], T> combine, params IReadOnlyEventValue<T>[] sources)
        {
            _sources = sources;
            _combine = combine;
            _comparer = EqualityComparer<T>.Default;

            _buffer = new T[sources.Length];

            for (int i = 0; i < sources.Length; i++)
                _buffer[i] = sources[i].Value;

            _value = combine(_buffer);

            foreach (var s in sources)
                s.Changed += OnSourceChanged;
        }
        public MultiEventValue(Func<T[], T> combine, IEqualityComparer<T> comparer, params IReadOnlyEventValue<T>[] sources)
        {
            _sources = sources;
            _combine = combine;
            _comparer = EqualityComparer<T>.Default;

            _buffer = new T[sources.Length];

            for (int i = 0; i < sources.Length; i++)
                _buffer[i] = sources[i].Value;

            _value = combine(_buffer);

            foreach (var s in sources)
                s.Changed += OnSourceChanged;
        }

        public void Dispose()
        {
            foreach (var s in _sources)
                s.Changed -= OnSourceChanged;
        }

        private void OnSourceChanged(T value)
        {
            for (int i = 0; i < _sources.Length; i++)
                _buffer[i] = _sources[i].Value;

            var newValue = _combine(_buffer);

            if (EqualityComparer<T>.Default.Equals(_value, newValue))
                return;

            _value = newValue;
            Changed?.Invoke(newValue);
        }

        public bool Equals(T otherValue)
            => _comparer.Equals(_value, otherValue);
    }
}
