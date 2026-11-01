using CatCode.StatefulEffects.Tests;
using Cysharp.Threading.Tasks;
using NUnit.Framework;

namespace CatCode.StatefulEffects.UniTaskTests
{
    public sealed class EffectActionWaitForStateAsyncTests
    {
        [Test]
        public void WaitForStateAsync_InState()
        {
            var fake = new FakeEffectAction(EffectActionState.Ready);

            Assert.AreEqual(EffectActionResult.AlreadyInState, Results.Of(fake.WaitForStateAsync(EffectActionState.Ready)));
        }

        [Test]
        public void WaitForStateAsync_ThenComplete()
        {
            var fake = new FakeEffectAction(EffectActionState.Playing);

            var task = fake.WaitForStateAsync(EffectActionState.Ready);
            Assert.AreEqual(UniTaskStatus.Pending, task.Status);
            fake.Completion();

            Assert.AreEqual(EffectActionResult.Completed, Results.Of(task));
        }

        [Test]
        public void WaitForStateAsync_FromNone()
        {
            var fake = new FakeEffectAction(EffectActionState.None);

            var task = fake.WaitForStateAsync(EffectActionState.Ready);
            Assert.AreEqual(UniTaskStatus.Pending, task.Status);
            fake.Prepare();

            Assert.AreEqual(EffectActionResult.Completed, Results.Of(task));
        }

        [Test]
        public void WaitForStateAsync_ThenStop()
        {
            var fake = new FakeEffectAction(EffectActionState.Playing);

            var task = fake.WaitForStateAsync(EffectActionState.Ready);
            fake.Stop();

            Assert.AreEqual(EffectActionResult.Stopped, Results.Of(task));
        }

        [Test]
        public void WaitForStateAsync_NoneTarget()
        {
            var fake = new FakeEffectAction(EffectActionState.Playing);

            var task = fake.WaitForStateAsync(EffectActionState.None);
            fake.Stop();

            Assert.AreEqual(EffectActionResult.Completed, Results.Of(task));
        }
    }
}
