using NUnit.Framework;

namespace CatCode.StatefulEffects.Tests
{
    public sealed class StateChangedTests
    {
        [Test]
        public void StateChanged_HideOnShowing()
        {
            var fake = new FakeShowHide(ShowHideState.Hidden);
            fake.OnStateChanged += state =>
            {
                if (state == ShowHideState.Showing)
                    fake.Hide();
            };

            fake.Show();

            Assert.AreEqual(ShowHideState.Hiding, fake.State);
            CollectionAssert.AreEqual(new[] { (Hook.OnHide, ShowHideState.Showing) }, fake.Hooks);
        }

        [Test]
        public void StateChanged_StopOnShown()
        {
            var fake = new FakeShowHide(ShowHideState.Hidden);
            fake.OnStateChanged += state =>
            {
                if (state == ShowHideState.Shown)
                    fake.Stop();
            };

            fake.SetShown();

            Assert.AreEqual(ShowHideState.None, fake.State);
            CollectionAssert.AreEqual(new[] { ShowHideState.Shown, ShowHideState.None }, fake.States);
        }
    }
}
