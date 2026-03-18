using CatCode.Common;
using CatCode.Pools;
using CatCode.Promises;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace System.Threading
{
    public static class EventValueUniTaskExtensions
    {
        public static async UniTask WaitAsync<T>(this IReadOnlyEventValue<T> eventValue, T targetValue, CancellationToken cancellationToken)
        {
            using var handle = EventValueEqualityConditionPool<T>.Get(out var condition);
            condition.Init(eventValue, targetValue, EqualityComparer<T>.Default);
            if (condition.Check())
            {
                await UniTask.CompletedTask;
                return;
            }
            var taskSource = EventValuePromise<T>.Create(eventValue, condition, cancellationToken, out var token);
            await new UniTask(taskSource, token);
        }

        public static async UniTask WaitAsync<T>(this IReadOnlyEventValue<T> eventValue, T targetValue, ComparisonType comparerType, CancellationToken cancellationToken)
        {
            using var handle = EventValueCompareConditionPool<T>.Get(out var condition);
            condition.Init(eventValue, targetValue, comparerType, Comparer<T>.Default);
            if (condition.Check())
            {
                await UniTask.CompletedTask;
                return;
            }
            var taskSource = EventValuePromise<T>.Create(eventValue, condition, cancellationToken, out var token);
            await new UniTask(taskSource, token);
        }

        public static UniTask WaitAsync<T>(this IReadOnlyEventValue<T> eventValue, ICondition condition, CancellationToken cancellationToken)
        {
            var taskSource = EventValuePromise<T>.Create(eventValue, condition, cancellationToken, out var token);
            return new UniTask(taskSource, token);
        }
    }
}