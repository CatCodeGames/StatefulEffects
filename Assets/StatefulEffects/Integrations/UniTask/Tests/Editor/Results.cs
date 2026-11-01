using Cysharp.Threading.Tasks;
using NUnit.Framework;

namespace CatCode.StatefulEffects.UniTaskTests
{
    internal static class Results
    {
        public static ShowHideResult Of(UniTask<ShowHideResult> task)
        {
            Assert.AreEqual(UniTaskStatus.Succeeded, task.Status);
            return task.GetAwaiter().GetResult();
        }

        public static EffectActionResult Of(UniTask<EffectActionResult> task)
        {
            Assert.AreEqual(UniTaskStatus.Succeeded, task.Status);
            return task.GetAwaiter().GetResult();
        }
    }
}
