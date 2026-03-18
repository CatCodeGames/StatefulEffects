using CatCode.Common;
using UnityEngine.Pool;

namespace CatCode.Pools
{
    public static class EventValueCompareConditionPool<T>
    {
        public static PooledObject<EventValueCompareCondition<T>> Get(out EventValueCompareCondition<T> condition)
            => s_comparePool.Get(out condition);

        private static readonly ObjectPool<EventValueCompareCondition<T>> s_comparePool = new(
            createFunc: () => new(),
            actionOnRelease: instance => instance.Reset());
    }

    public static class EventValueEqualityConditionPool<T>
    {
        private static readonly ObjectPool<EventValueEqualityCondition<T>> s_equalityPool = new(
            createFunc: () => new(),
            actionOnRelease: instance => instance.Reset());

        public static PooledObject<EventValueEqualityCondition<T>> Get(out EventValueEqualityCondition<T> condition)
            => s_equalityPool.Get(out condition);
    }
}