using CatCode.StatefulEffects.Tests;
using Cysharp.Threading.Tasks;
using NUnit.Framework;

namespace CatCode.StatefulEffects.UniTaskTests
{
    public sealed class HideAsyncTests
    {
        [Test]
        public void HideAsync_FromShown()
        {
            var fake = new FakeShowHide(ShowHideState.Shown);

            var task = fake.HideAsync();
            Assert.AreEqual(UniTaskStatus.Pending, task.Status);
            fake.Completion();

            Assert.AreEqual(ShowHideResult.Completed, Results.Of(task));
        }

        [Test]
        public void HideAsync_FromHidden()
        {
            var fake = new FakeShowHide(ShowHideState.Hidden);

            Assert.AreEqual(ShowHideResult.AlreadyInState, Results.Of(fake.HideAsync()));
        }

        [Test]
        public void HideAsync_WhileHiding()
        {
            var fake = new FakeShowHide(ShowHideState.Hiding);

            var task = fake.HideAsync();
            Assert.AreEqual(UniTaskStatus.Pending, task.Status);
            fake.Completion();

            Assert.AreEqual(ShowHideResult.Completed, Results.Of(task));
        }

        [Test]
        public void HideAsync_WhileShowing_WithShowOnInterruptIgnore()
        {
            var fake = new FakeShowHide(ShowHideState.Showing, Policy.ShowOnInterruptIgnore);

            Assert.AreEqual(ShowHideResult.Rejected, Results.Of(fake.HideAsync()));
        }

        [Test]
        public void HideAsync_ThenShow()
        {
            var fake = new FakeShowHide(ShowHideState.Shown);

            var task = fake.HideAsync();
            fake.Show();

            Assert.AreEqual(ShowHideResult.Interrupted, Results.Of(task));
        }

        [Test]
        public void HideAsync_ThenStop()
        {
            var fake = new FakeShowHide(ShowHideState.Shown);

            var task = fake.HideAsync();
            fake.Stop();

            Assert.AreEqual(ShowHideResult.Stopped, Results.Of(task));
        }

        [Test]
        public void HideAsync_UntilReached_ThenShow()
        {
            var fake = new FakeShowHide(ShowHideState.Shown);

            var task = fake.HideAsync(ShowHideWaitMode.UntilReached);
            fake.Show();
            Assert.AreEqual(UniTaskStatus.Pending, task.Status);
            fake.Hide();
            fake.Completion();

            Assert.AreEqual(ShowHideResult.Completed, Results.Of(task));
        }
    }
}
