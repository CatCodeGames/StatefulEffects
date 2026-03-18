using System;
using System.Collections.Generic;

namespace CatCode.Common
{
    public sealed class EventValue<T> : IEventValue<T>
    {
        private T _value;
        private readonly IEqualityComparer<T> _comparer;
        private readonly bool _checkEquals;

        public T Value
        {
            get => _value;
            set
            {
                if (_checkEquals && _comparer.Equals(_value, value))
                    return;

                _value = value;
                Changed?.Invoke(value);
            }
        }

        public event Action<T> Changed;

        public EventValue(T initial)
        {
            _value = initial;
            _comparer = EqualityComparer<T>.Default;
            _checkEquals = true;
        }

        public EventValue(T initial, bool checkEquals)
        {
            _value = initial;
            _comparer = EqualityComparer<T>.Default;
            _checkEquals = true;
        }

        public EventValue(T initial, IEqualityComparer<T> comparer)
        {
            _value = initial;
            _comparer = comparer ?? EqualityComparer<T>.Default;
            _checkEquals = true;
        }

        public void SetSilently(T newValue)
        {
            _value = newValue;
        }

        public void ForceNotify()
        {
            Changed?.Invoke(_value);
        }

        public bool Equals(T otherValue)
            => _comparer.Equals(otherValue, _value);
    }
}
