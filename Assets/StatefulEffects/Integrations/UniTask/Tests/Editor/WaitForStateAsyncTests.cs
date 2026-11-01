using CatCode.StatefulEffects.Tests;
using Cysharp.Threading.Tasks;
using NUnit.Framework;

namespace CatCode.StatefulEffects.UniTaskTests
{
    public sealed class WaitForStateAsyncTests
    {
        [Test]
        public void WaitForStateAsync_InState()
        {
            var fake = new FakeShowHide(ShowHideState.Shown);

            Assert.AreEqual(ShowHideResult.AlreadyInState, Results.Of(fake.WaitForStateAsync(ShowHideState.Shown)));
        }

        [Test]
        public void WaitForStateAsync_ThenShow()
        {
            var fake = new FakeShowHide(ShowHideState.Hidden);

            var task = fake.WaitForStateAsync(ShowHideState.Shown);
            fake.Show();
            Assert.AreEqual(UniTaskStatus.Pending, task.Status);
            fake.Completion();

            Assert.AreEqual(ShowHideResult.Completed, Results.Of(task));
        }

        [Test]
        public void WaitForStateAsync_FromNone()
        {
            var fake = new FakeShowHide(ShowHideState.None);

            var task = fake.WaitForStateAsync(ShowHideState.Shown);
            Assert.AreEqual(UniTaskStatus.Pending, task.Status);
            fake.SetShown();

            Assert.AreEqual(ShowHideResult.Completed, Results.Of(task));
        }

        [Test]
        public void WaitForStateAsync_ThenStop()
        {
            var fake = new FakeShowHide(ShowHideState.Hidden);

            var task = fake.WaitForStateAsync(ShowHideState.Shown);
            fake.Show();
            fake.Stop();

            Assert.AreEqual(ShowHideResult.Stopped, Results.Of(task));
        }

        [Test]
        public void WaitForStateAsync_NoneTarget()
        {
            var fake = new FakeShowHide(ShowHideState.Shown);

            var task = fake.WaitForStateAsync(ShowHideState.None);
            fake.Stop();

            Assert.AreEqual(ShowHideResult.Completed, Results.Of(task));
        }
    }
}
