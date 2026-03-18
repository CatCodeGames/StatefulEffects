using System.Collections.Generic;

namespace CatCode.Common
{
    public sealed class EventValueCompareCondition<T> : ICondition
    {
        private IReadOnlyEventValue<T> _eventValue;
        private T _targetValue;
        private ComparisonType _comparisonType;
        private IComparer<T> _comparer;

        public void Init(
            IReadOnlyEventValue<T> eventValue,
            T targetValue,
            ComparisonType comparisonType,
            IComparer<T> comparer)
        {
            _eventValue = eventValue;
            _targetValue = targetValue;
            _comparisonType = comparisonType;
            _comparer = comparer;
        }

        public void Reset()
        {
            _eventValue = null;
            _targetValue = default;
            _comparisonType = ComparisonType.Equal;
            _comparer = null;
        }

        public bool Check()
        {
            int cmp = _comparer.Compare(_eventValue.Value, _targetValue);

            return _comparisonType switch
            {
                ComparisonType.Equal => cmp == 0,
                ComparisonType.NotEqual => cmp != 0,
                ComparisonType.Greater => cmp > 0,
                ComparisonType.Less => cmp < 0,
                ComparisonType.GreaterOrEqual => cmp >= 0,
                ComparisonType.LessOrEqual => cmp <= 0,
                _ => false
            };
        }
    }

}