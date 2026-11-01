using NUnit.Framework;

namespace CatCode.StatefulEffects.Tests
{
    public sealed class CompletionTests
    {
        [Test]
        public void Show_ThenComplete()
        {
            var fake = new FakeShowHide(ShowHideState.Hidden);

            fake.Show();
            fake.Completion();

            Assert.AreEqual(ShowHideState.Shown, fake.State);
            CollectionAssert.AreEqual(new[] { ShowHideState.Showing, ShowHideState.Shown }, fake.States);
        }

        [Test]
        public void Hide_ThenComplete()
        {
            var fake = new FakeShowHide(ShowHideState.Shown);

            fake.Hide();
            fake.Completion();

            Assert.AreEqual(ShowHideState.Hidden, fake.State);
            CollectionAssert.AreEqual(new[] { ShowHideState.Hiding, ShowHideState.Hidden }, fake.States);
        }

        [Test]
        public void Show_InstantCompletion()
        {
            var fake = new FakeShowHide(ShowHideState.Hidden);
            fake.CompleteInstantly = true;

            fake.Show();

            Assert.AreEqual(ShowHideState.Shown, fake.State);
            CollectionAssert.AreEqual(new[] { ShowHideState.Showing, ShowHideState.Shown }, fake.States);
        }

        [Test]
        public void Hide_InstantCompletion()
        {
            var fake = new FakeShowHide(ShowHideState.Shown);
            fake.CompleteInstantly = true;

            fake.Hide();

            Assert.AreEqual(ShowHideState.Hidden, fake.State);
            CollectionAssert.AreEqual(new[] { ShowHideState.Hiding, ShowHideState.Hidden }, fake.States);
        }

        [Test]
        public void Complete_AfterInterrupt()
        {
            var fake = new FakeShowHide(ShowHideState.Hidden);
            fake.Show();
            var showCompletion = fake.Completion;
            fake.Hide();

            showCompletion();

            Assert.AreEqual(ShowHideState.Hiding, fake.State);
        }

        [Test]
        public void Complete_AfterSetState()
        {
            var fake = new FakeShowHide(ShowHideState.Hidden);
            fake.Show();
            fake.SetHidden();
            fake.States.Clear();

            fake.Completion();

            Assert.AreEqual(ShowHideState.Hidden, fake.State);
            CollectionAssert.IsEmpty(fake.States);
        }

        [Test]
        public void Complete_AfterStop()
        {
            var fake = new FakeShowHide(ShowHideState.Hidden);
            fake.Show();
            fake.Stop();

            fake.Completion();

            Assert.AreEqual(ShowHideState.None, fake.State);
        }

        [Test]
        public void Complete_AfterRestart()
        {
            var fake = new FakeShowHide(ShowHideState.Shown, Policy.ShowOnRepeatRestart);
            fake.Show();
            var firstCompletion = fake.Completion;
            // Restart действует только из Shown, поэтому возвращаемся в него, не завершая первый показ.
            fake.SetShown();
            fake.Show();
            fake.States.Clear();

            firstCompletion();

            Assert.AreEqual(ShowHideState.Showing, fake.State);
            CollectionAssert.IsEmpty(fake.States);
        }

        [Test]
        public void Complete_Twice()
        {
            var fake = new FakeShowHide(ShowHideState.Hidden);
            fake.Show();
            var completion = fake.Completion;

            completion();
            completion();

            CollectionAssert.AreEqual(new[] { ShowHideState.Showing, ShowHideState.Shown }, fake.States);
        }
    }
}
