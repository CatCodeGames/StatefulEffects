using System.Collections.Generic;
using NUnit.Framework;

using static CatCode.StatefulEffects.EffectActionState;
using static CatCode.StatefulEffects.Tests.Hook;
using static CatCode.StatefulEffects.Tests.ActionPolicy;
using static CatCode.StatefulEffects.Tests.EffectActionTransitionTableTests.Command;

namespace CatCode.StatefulEffects.Tests
{
    public sealed class EffectActionTransitionTableTests
    {
        public enum Command { Play, Prepare, Stop }

        private static IEnumerable<TestCaseData> PlayTable()
        {
            //                command from     policy          to       hooks
            yield return Case(Play,   None,    Default,        Playing, (OnPlay, None));
            yield return Case(Play,   Ready,   Default,        Playing, (OnPlay, Ready));
            yield return Case(Play,   Playing, Default,        Playing, (OnPlay, Playing));
            yield return Case(Play,   Playing, OnRepeatIgnore, Playing);
        }

        private static IEnumerable<TestCaseData> PrepareTable()
        {
            //                command  from     to     hooks
            yield return Case(Prepare, None,    Ready, (OnPrepare, None));
            yield return Case(Prepare, Ready,   Ready);
            yield return Case(Prepare, Playing, Ready, (OnPrepare, Playing));
        }

        private static IEnumerable<TestCaseData> StopTable()
        {
            //                command from     to    hooks
            yield return Case(Stop,   None,    None, (OnStop, None));
            yield return Case(Stop,   Ready,   None, (OnStop, Ready));
            yield return Case(Stop,   Playing, None, (OnStop, Playing));
        }

        [TestCaseSource(nameof(PlayTable))]
        [TestCaseSource(nameof(PrepareTable))]
        [TestCaseSource(nameof(StopTable))]
        public void Transition(Command command, EffectActionState from, ActionPolicy policy, EffectActionState to, (Hook, EffectActionState)[] hooks)
        {
            var fake = new FakeEffectAction(from, policy);

            switch (command)
            {
                case Play:
                    fake.Play();
                    break;
                case Prepare:
                    fake.Prepare();
                    break;
                case Stop:
                    fake.Stop();
                    break;
            }

            var expectedStateChanges = from == to ? new EffectActionState[0] : new[] { to };
            Assert.AreEqual(to, fake.State);
            CollectionAssert.AreEqual(hooks, fake.Hooks);
            CollectionAssert.AreEqual(expectedStateChanges, fake.States);
        }

        private static TestCaseData Case(Command command, EffectActionState from, EffectActionState to, params (Hook, EffectActionState)[] hooks)
            => Case(command, from, Default, to, hooks);

        private static TestCaseData Case(Command command, EffectActionState from, ActionPolicy policy, EffectActionState to, params (Hook, EffectActionState)[] hooks)
        {
            var prefix = from == Playing ? "While" : "From";
            var suffix = policy == Default ? "" : $"_With{policy}";
            return new TestCaseData(command, from, policy, to, hooks).SetName($"{command}_{prefix}{from}{suffix}");
        }
    }
}
