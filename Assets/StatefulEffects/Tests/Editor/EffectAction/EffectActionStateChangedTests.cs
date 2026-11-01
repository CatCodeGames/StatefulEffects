using NUnit.Framework;

namespace CatCode.StatefulEffects.Tests
{
    public sealed class EffectActionStateChangedTests
    {
        [Test]
        public void StateChanged_PrepareOnPlaying()
        {
            var fake = new FakeEffectAction(EffectActionState.Ready);
            fake.OnStateChanged += state =>
            {
                if (state == EffectActionState.Playing)
                    fake.Prepare();
            };

            fake.Play();

            Assert.AreEqual(EffectActionState.Ready, fake.State);
            CollectionAssert.AreEqual(new[] { (Hook.OnPrepare, EffectActionState.Playing) }, fake.Hooks);
        }

        [Test]
        public void StateChanged_PlayOnReady()
        {
            var fake = new FakeEffectAction(EffectActionState.Playing);
            fake.OnStateChanged += state =>
            {
                if (state == EffectActionState.Ready)
                    fake.Play();
            };

            fake.Completion();

            Assert.AreEqual(EffectActionState.Playing, fake.State);
            CollectionAssert.AreEqual(new[] { (Hook.OnPlay, EffectActionState.Ready) }, fake.Hooks);
            CollectionAssert.AreEqual(new[] { EffectActionState.Ready, EffectActionState.Playing }, fake.States);
        }

        [Test]
        public void StateChanged_StopOnReady()
        {
            var fake = new FakeEffectAction(EffectActionState.Playing);
            fake.OnStateChanged += state =>
            {
                if (state == EffectActionState.Ready)
                    fake.Stop();
            };

            fake.Completion();

            Assert.AreEqual(EffectActionState.None, fake.State);
            CollectionAssert.AreEqual(new[] { EffectActionState.Ready, EffectActionState.None }, fake.States);
        }
    }
}
