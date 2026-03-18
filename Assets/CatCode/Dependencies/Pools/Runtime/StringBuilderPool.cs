using System.Text;
using UnityEngine.Pool;

namespace CatCode.Pools
{
    public class StringBuilderPool
    {
        private static readonly ObjectPool<StringBuilder> s_Pool = new(
            createFunc: () => new StringBuilder(),
            actionOnGet: instance => instance.Clear());

        public static PooledObject<StringBuilder> Get(out StringBuilder stringBuilder)
            => s_Pool.Get(out stringBuilder);

        public static StringBuilder Get()
            => s_Pool.Get();

        public static void Release(StringBuilder toRelease)
            => s_Pool.Release(toRelease);
    }
}