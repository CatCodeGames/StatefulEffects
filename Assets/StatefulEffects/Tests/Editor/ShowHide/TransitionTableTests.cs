using System.Collections.Generic;
using NUnit.Framework;

using static CatCode.StatefulEffects.ShowHideState;
using static CatCode.StatefulEffects.Tests.Hook;
using static CatCode.StatefulEffects.Tests.Policy;
using static CatCode.StatefulEffects.Tests.TransitionTableTests.Command;

namespace CatCode.StatefulEffects.Tests
{
    public sealed class TransitionTableTests
    {
        public enum Command { Show, Hide, SetShown, SetHidden, Stop }

        private static IEnumerable<TestCaseData> ShowTable()
        {
            //                command  from     policy                 to       hooks
            yield return Case(Show,    None,    Default,               Showing, (OnShow, None));
            yield return Case(Show,    Hidden,  Default,               Showing, (OnShow, Hidden));
            yield return Case(Show,    Showing, Default,               Showing);
            yield return Case(Show,    Shown,   Default,               Shown);
            yield return Case(Show,    Shown,   ShowOnRepeatRestart,   Showing, (OnSetHidden, Shown), (OnShow, Hidden));
            yield return Case(Show,    Shown,   HideOnRepeatRestart,   Shown);
            yield return Case(Show,    Hiding,  Default,               Showing, (OnShow, Hiding));
            yield return Case(Show,    Hiding,  HideOnInterruptIgnore, Hiding);
            yield return Case(Show,    Hiding,  ShowOnInterruptIgnore, Showing, (OnShow, Hiding));
        }

        private static IEnumerable<TestCaseData> HideTable()
        {
            //                command  from     policy                 to       hooks
            yield return Case(Hide,    None,    Default,               Hiding,  (OnHide, None));
            yield return Case(Hide,    Shown,   Default,               Hiding,  (OnHide, Shown));
            yield return Case(Hide,    Hiding,  Default,               Hiding);
            yield return Case(Hide,    Hidden,  Default,               Hidden);
            yield return Case(Hide,    Hidden,  HideOnRepeatRestart,   Hiding,  (OnSetShown, Hidden), (OnHide, Shown));
            yield return Case(Hide,    Hidden,  ShowOnRepeatRestart,   Hidden);
            yield return Case(Hide,    Showing, Default,               Hiding,  (OnHide, Showing));
            yield return Case(Hide,    Showing, ShowOnInterruptIgnore, Showing);
            yield return Case(Hide,    Showing, HideOnInterruptIgnore, Hiding,  (OnHide, Showing));
        }

        private static IEnumerable<TestCaseData> SetShownTable()
        {
            //                command   from     to     hooks
            yield return Case(SetShown, None,    Shown, (OnSetShown, None));
            yield return Case(SetShown, Hidden,  Shown, (OnSetShown, Hidden));
            yield return Case(SetShown, Showing, Shown, (OnSetShown, Showing));
            yield return Case(SetShown, Shown,   Shown);
            yield return Case(SetShown, Hiding,  Shown, (OnSetShown, Hiding));
        }

        private static IEnumerable<TestCaseData> SetHiddenTable()
        {
            //                command    from     to      hooks
            yield return Case(SetHidden, None,    Hidden, (OnSetHidden, None));
            yield return Case(SetHidden, Hidden,  Hidden);
            yield return Case(SetHidden, Showing, Hidden, (OnSetHidden, Showing));
            yield return Case(SetHidden, Shown,   Hidden, (OnSetHidden, Shown));
            yield return Case(SetHidden, Hiding,  Hidden, (OnSetHidden, Hiding));
        }

        private static IEnumerable<TestCaseData> StopTable()
        {
            //                command from     to    hooks
            yield return Case(Stop,   None,    None, (OnStop, None));
            yield return Case(Stop,   Hidden,  None, (OnStop, Hidden));
            yield return Case(Stop,   Showing, None, (OnStop, Showing));
            yield return Case(Stop,   Shown,   None, (OnStop, Shown));
            yield return Case(Stop,   Hiding,  None, (OnStop, Hiding));
        }

        [TestCaseSource(nameof(ShowTable))]
        [TestCaseSource(nameof(HideTable))]
        [TestCaseSource(nameof(SetShownTable))]
        [TestCaseSource(nameof(SetHiddenTable))]
        [TestCaseSource(nameof(StopTable))]
        public void Transition(Command command, ShowHideState from, Policy policy, ShowHideState to, (Hook, ShowHideState)[] hooks)
        {
            var fake = new FakeShowHide(from, policy);

            switch (command)
            {
                case Show:
                    fake.Show();
                    break;
                case Hide:
                    fake.Hide();
                    break;
                case SetShown:
                    fake.SetShown();
                    break;
                case SetHidden:
                    fake.SetHidden();
                    break;
                case Stop:
                    fake.Stop();
                    break;
            }

            var expectedStateChanges = from == to ? new ShowHideState[0] : new[] { to };
            Assert.AreEqual(to, fake.State);
            CollectionAssert.AreEqual(hooks, fake.Hooks);
            CollectionAssert.AreEqual(expectedStateChanges, fake.States);
        }

        private static TestCaseData Case(Command command, ShowHideState from, ShowHideState to, params (Hook, ShowHideState)[] hooks)
            => Case(command, from, Default, to, hooks);

        private static TestCaseData Case(Command command, ShowHideState from, Policy policy, ShowHideState to, params (Hook, ShowHideState)[] hooks)
        {
            var prefix = from == Showing || from == Hiding ? "While" : "From";
            var suffix = policy == Default ? "" : $"_With{policy}";
            return new TestCaseData(command, from, policy, to, hooks).SetName($"{command}_{prefix}{from}{suffix}");
        }
    }
}
