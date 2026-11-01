using System;
using System.Threading;
using CatCode.StatefulEffects.Tests;
using Cysharp.Threading.Tasks;
using NUnit.Framework;

namespace CatCode.StatefulEffects.UniTaskTests
{
    public sealed class PlayAsyncTests
    {
        [Test]
        public void PlayAsync_FromReady()
        {
            var fake = new FakeEffectAction(EffectActionState.Ready);

            var task = fake.PlayAsync();
            Assert.AreEqual(UniTaskStatus.Pending, task.Status);
            fake.Completion();

            Assert.AreEqual(EffectActionResult.Completed, Results.Of(task));
        }

        [Test]
        public void PlayAsync_FromNone()
        {
            var fake = new FakeEffectAction(EffectActionState.None);

            var task = fake.PlayAsync();
            Assert.AreEqual(UniTaskStatus.Pending, task.Status);
            fake.Completion();

            Assert.AreEqual(EffectActionResult.Completed, Results.Of(task));
        }

        [Test]
        public void PlayAsync_InstantCompletion()
        {
            var fake = new FakeEffectAction(EffectActionState.Ready);
            fake.CompleteInstantly = true;

            Assert.AreEqual(EffectActionResult.Completed, Results.Of(fake.PlayAsync()));
        }

        [Test]
        public void PlayAsync_WhilePlaying()
        {
            var fake = new FakeEffectAction(EffectActionState.Playing);
            var firstCompletion = fake.Completion;

            var task = fake.PlayAsync();
            firstCompletion();
            Assert.AreEqual(UniTaskStatus.Pending, task.Status);
            fake.Completion();

            Assert.AreEqual(EffectActionResult.Completed, Results.Of(task));
        }

        [Test]
        public void PlayAsync_WhilePlaying_WithOnRepeatIgnore()
        {
            var fake = new FakeEffectAction(EffectActionState.Playing, ActionPolicy.OnRepeatIgnore);

            var task = fake.PlayAsync();
            Assert.AreEqual(UniTaskStatus.Pending, task.Status);
            fake.Completion();

            Assert.AreEqual(EffectActionResult.Completed, Results.Of(task));
        }

        [Test]
        public void PlayAsync_ThenPlay()
        {
            var fake = new FakeEffectAction(EffectActionState.Ready);

            var task = fake.PlayAsync();
            fake.Play();
            Assert.AreEqual(UniTaskStatus.Pending, task.Status);
            fake.Completion();

            Assert.AreEqual(EffectActionResult.Completed, Results.Of(task));
        }

        [Test]
        public void PlayAsync_ThenPrepare()
        {
            var fake = new FakeEffectAction(EffectActionState.Ready);

            var task = fake.PlayAsync();
            fake.Prepare();

            Assert.AreEqual(EffectActionResult.Completed, Results.Of(task));
        }

        [Test]
        public void PlayAsync_ThenStop()
        {
            var fake = new FakeEffectAction(EffectActionState.Ready);

            var task = fake.PlayAsync();
            fake.Stop();

            Assert.AreEqual(EffectActionResult.Stopped, Results.Of(task));
        }

        [Test]
        public void PlayAsync_StopOnPlaying()
        {
            var fake = new FakeEffectAction(EffectActionState.Ready);
            fake.OnStateChanged += state =>
            {
                if (state == EffectActionState.Playing)
                    fake.Stop();
            };

            Assert.AreEqual(EffectActionResult.Stopped, Results.Of(fake.PlayAsync()));
        }

        [Test]
        public void PlayAsync_ThenCancel()
        {
            var fake = new FakeEffectAction(EffectActionState.Ready);
            using var cts = new CancellationTokenSource();

            var task = fake.PlayAsync(cts.Token);
            cts.Cancel();

            Assert.AreEqual(UniTaskStatus.Canceled, task.Status);
            Assert.Throws<OperationCanceledException>(() => task.GetAwaiter().GetResult());
            Assert.AreEqual(EffectActionState.Playing, fake.State);
        }

        [Test]
        public void PlayAsync_CanceledToken()
        {
            var fake = new FakeEffectAction(EffectActionState.Ready);
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            var task = fake.PlayAsync(cts.Token);

            Assert.AreEqual(UniTaskStatus.Canceled, task.Status);
            Assert.AreEqual(EffectActionState.Ready, fake.State);
            CollectionAssert.IsEmpty(fake.Hooks);
        }
    }
}
