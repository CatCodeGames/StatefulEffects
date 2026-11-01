using NUnit.Framework;

namespace CatCode.StatefulEffects.Tests
{
    public sealed class EffectActionDefaultsTests
    {
        [Test]
        public void EffectActionState_Default()
        {
            Assert.AreEqual(EffectActionState.None, default(EffectActionState));
        }

        [Test]
        public void EffectActionPolicy_Default()
        {
            Assert.AreEqual(InterruptResolution.Accept, EffectActionPolicy.Default.OnRepeat);
        }
    }
}
