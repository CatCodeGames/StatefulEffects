using System.Collections.Generic;
using UnityEngine.Pool;

namespace UnityEngine.Pools 
{
    public class QueuePool<T>
    {
        private static readonly ObjectPool<Queue<T>> s_Pool = new(
            createFunc: () => new Queue<T>(),
            actionOnGet: null,
            actionOnRelease: stack => stack.Clear());

        public static PooledObject<Queue<T>> Get(out Queue<T> value)
            => s_Pool.Get(out value);

        public static Queue<T> Get()
            => s_Pool.Get();

        public static void Release(Queue<T> toRelease)
            => s_Pool.Release(toRelease);
    }
}