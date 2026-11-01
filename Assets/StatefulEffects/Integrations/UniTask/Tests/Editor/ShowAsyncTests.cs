using System;
using System.Threading;
using CatCode.StatefulEffects.Tests;
using Cysharp.Threading.Tasks;
using NUnit.Framework;

namespace CatCode.StatefulEffects.UniTaskTests
{
    public sealed class ShowAsyncTests
    {
        [Test]
        public void ShowAsync_FromHidden()
        {
            var fake = new FakeShowHide(ShowHideState.Hidden);

            var task = fake.ShowAsync();
            Assert.AreEqual(UniTaskStatus.Pending, task.Status);
            fake.Completion();

            Assert.AreEqual(ShowHideResult.Completed, Results.Of(task));
        }

        [Test]
        public void ShowAsync_InstantCompletion()
        {
            var fake = new FakeShowHide(ShowHideState.Hidden);
            fake.CompleteInstantly = true;

            Assert.AreEqual(ShowHideResult.Completed, Results.Of(fake.ShowAsync()));
        }

        [Test]
        public void ShowAsync_FromShown()
        {
            var fake = new FakeShowHide(ShowHideState.Shown);

            Assert.AreEqual(ShowHideResult.AlreadyInState, Results.Of(fake.ShowAsync()));
        }

        [Test]
        public void ShowAsync_FromShown_WithShowOnRepeatRestart()
        {
            var fake = new FakeShowHide(ShowHideState.Shown, Policy.ShowOnRepeatRestart);

            var task = fake.ShowAsync();
            Assert.AreEqual(UniTaskStatus.Pending, task.Status);
            fake.Completion();

            Assert.AreEqual(ShowHideResult.Completed, Results.Of(task));
        }

        [Test]
        public void ShowAsync_WhileShowing()
        {
            var fake = new FakeShowHide(ShowHideState.Showing);

            var task = fake.ShowAsync();
            Assert.AreEqual(UniTaskStatus.Pending, task.Status);
            fake.Completion();

            Assert.AreEqual(ShowHideResult.Completed, Results.Of(task));
        }

        [Test]
        public void ShowAsync_WhileHiding_WithHideOnInterruptIgnore()
        {
            var fake = new FakeShowHide(ShowHideState.Hiding, Policy.HideOnInterruptIgnore);

            Assert.AreEqual(ShowHideResult.Rejected, Results.Of(fake.ShowAsync()));
        }

        [Test]
        public void ShowAsync_ThenHide()
        {
            var fake = new FakeShowHide(ShowHideState.Hidden);

            var task = fake.ShowAsync();
            fake.Hide();

            Assert.AreEqual(ShowHideResult.Interrupted, Results.Of(task));
        }

        [Test]
        public void ShowAsync_ThenSetHidden()
        {
            var fake = new FakeShowHide(ShowHideState.Hidden);

            var task = fake.ShowAsync();
            fake.SetHidden();

            Assert.AreEqual(ShowHideResult.Interrupted, Results.Of(task));
        }

        [Test]
        public void ShowAsync_ThenSetShown()
        {
            var fake = new FakeShowHide(ShowHideState.Hidden);

            var task = fake.ShowAsync();
            fake.SetShown();

            Assert.AreEqual(ShowHideResult.Completed, Results.Of(task));
        }

        [Test]
        public void ShowAsync_ThenStop()
        {
            var fake = new FakeShowHide(ShowHideState.Hidden);

            var task = fake.ShowAsync();
            fake.Stop();

            Assert.AreEqual(ShowHideResult.Stopped, Results.Of(task));
        }

        [Test]
        public void ShowAsync_UntilReached_ThenHide()
        {
            var fake = new FakeShowHide(ShowHideState.Hidden);

            var task = fake.ShowAsync(ShowHideWaitMode.UntilReached);
            fake.Hide();
            fake.Completion();
            Assert.AreEqual(UniTaskStatus.Pending, task.Status);
            fake.Show();
            fake.Completion();

            Assert.AreEqual(ShowHideResult.Completed, Results.Of(task));
        }

        [Test]
        public void ShowAsync_UntilReached_ThenStop()
        {
            var fake = new FakeShowHide(ShowHideState.Hidden);

            var task = fake.ShowAsync(ShowHideWaitMode.UntilReached);
            fake.Hide();
            fake.Stop();

            Assert.AreEqual(ShowHideResult.Stopped, Results.Of(task));
        }

        [Test]
        public void ShowAsync_ThenCancel()
        {
            var fake = new FakeShowHide(ShowHideState.Hidden);
            using var cts = new CancellationTokenSource();

            var task = fake.ShowAsync(cts.Token);
            cts.Cancel();

            Assert.AreEqual(UniTaskStatus.Canceled, task.Status);
            Assert.Throws<OperationCanceledException>(() => task.GetAwaiter().GetResult());
            Assert.AreEqual(ShowHideState.Showing, fake.State);
        }

        [Test]
        public void ShowAsync_CanceledToken()
        {
            var fake = new FakeShowHide(ShowHideState.Hidden);
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            var task = fake.ShowAsync(cts.Token);

            Assert.AreEqual(UniTaskStatus.Canceled, task.Status);
            Assert.AreEqual(ShowHideState.Hidden, fake.State);
        }
    }
}
