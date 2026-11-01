using NUnit.Framework;

namespace CatCode.StatefulEffects.Tests
{
    public sealed class EffectActionCompletionTests
    {
        [Test]
        public void Play_ThenComplete()
        {
            var fake = new FakeEffectAction(EffectActionState.Ready);

            fake.Play();
            fake.Completion();

            Assert.AreEqual(EffectActionState.Ready, fake.State);
            CollectionAssert.AreEqual(new[] { EffectActionState.Playing, EffectActionState.Ready }, fake.States);
        }

        [Test]
        public void Play_InstantCompletion()
        {
            var fake = new FakeEffectAction(EffectActionState.Ready);
            fake.CompleteInstantly = true;

            fake.Play();

            Assert.AreEqual(EffectActionState.Ready, fake.State);
            CollectionAssert.AreEqual(new[] { EffectActionState.Playing, EffectActionState.Ready }, fake.States);
        }

        [Test]
        public void Complete_AfterRepeat()
        {
            var fake = new FakeEffectAction(EffectActionState.Playing);
            var firstCompletion = fake.Completion;
            fake.Play();

            firstCompletion();

            Assert.AreEqual(EffectActionState.Playing, fake.State);
            CollectionAssert.IsEmpty(fake.States);
        }

        [Test]
        public void Complete_AfterRepeat_WithOnRepeatIgnore()
        {
            var fake = new FakeEffectAction(EffectActionState.Playing, ActionPolicy.OnRepeatIgnore);
            var completion = fake.Completion;
            fake.Play();

            completion();

            Assert.AreEqual(EffectActionState.Ready, fake.State);
            CollectionAssert.AreEqual(new[] { EffectActionState.Ready }, fake.States);
        }

        [Test]
        public void Complete_AfterReplay()
        {
            var fake = new FakeEffectAction(EffectActionState.Ready);
            fake.Play();
            var firstCompletion = fake.Completion;
            firstCompletion();
            fake.Play();
            fake.States.Clear();

            firstCompletion();

            Assert.AreEqual(EffectActionState.Playing, fake.State);
            CollectionAssert.IsEmpty(fake.States);
        }

        [Test]
        public void Complete_AfterPrepare()
        {
            var fake = new FakeEffectAction(EffectActionState.Ready);
            fake.Play();
            fake.Prepare();
            fake.States.Clear();

            fake.Completion();

            Assert.AreEqual(EffectActionState.Ready, fake.State);
            CollectionAssert.IsEmpty(fake.States);
        }

        [Test]
        public void Complete_AfterStop()
        {
            var fake = new FakeEffectAction(EffectActionState.Ready);
            fake.Play();
            fake.Stop();

            fake.Completion();

            Assert.AreEqual(EffectActionState.None, fake.State);
        }

        [Test]
        public void Complete_Twice()
        {
            var fake = new FakeEffectAction(EffectActionState.Ready);
            fake.Play();
            var completion = fake.Completion;

            completion();
            completion();

            CollectionAssert.AreEqual(new[] { EffectActionState.Playing, EffectActionState.Ready }, fake.States);
        }
    }
}
