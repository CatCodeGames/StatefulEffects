using System.Collections.Generic;

namespace CatCode.Common
{
    public sealed class EventValueEqualityCondition<T> : ICondition
    {
        private IReadOnlyEventValue<T> _eventValue;
        private T _targetValue;
        private IEqualityComparer<T> _comparer;

        public void Init(IReadOnlyEventValue<T> eventValue, T targetValue, IEqualityComparer<T> comparer)
        {
            _eventValue = eventValue;
            _targetValue = targetValue;
            _comparer = comparer;
        }

        public void Reset()
        {
            _eventValue = null;
            _targetValue = default;
            _comparer = null;
        }

        public bool Check()
            => _comparer.Equals(_eventValue.Value, _targetValue);
    }
}