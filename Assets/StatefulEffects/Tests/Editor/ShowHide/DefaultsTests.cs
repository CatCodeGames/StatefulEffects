using NUnit.Framework;

namespace CatCode.StatefulEffects.Tests
{
    public sealed class DefaultsTests
    {
        [Test]
        public void ShowHideState_Default()
        {
            Assert.AreEqual(ShowHideState.None, default(ShowHideState));
        }

        [Test]
        public void ShowHidePolicy_Default()
        {
            var policy = ShowHidePolicy.Default;

            Assert.AreEqual(InterruptResolution.Accept, policy.Show.OnInterrupt);
            Assert.AreEqual(InterruptResolution.Accept, policy.Hide.OnInterrupt);
            Assert.AreEqual(RepeatResolution.Ignore, policy.Show.OnRepeat);
            Assert.AreEqual(RepeatResolution.Ignore, policy.Hide.OnRepeat);
        }
    }
}
