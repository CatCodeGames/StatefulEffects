using System;
using System.Collections.Generic;

namespace CatCode.StatefulEffects.Tests
{
    public sealed class FakeEffectAction : EffectAction
    {
        public readonly List<(Hook Hook, EffectActionState From)> Hooks = new();
        public readonly List<EffectActionState> States = new();

        private readonly EffectActionPolicy _policy;

        public Action Completion { get; private set; }
        public bool CompleteInstantly { get; set; }

        protected override EffectActionPolicy Policy => _policy;

        public FakeEffectAction(EffectActionState state, ActionPolicy policy = default)
        {
            _policy = ActionPolicies.Get(policy);
            switch (state)
            {
                case EffectActionState.None:
                    break;
                case EffectActionState.Ready:
                    Prepare();
                    break;
                case EffectActionState.Playing:
                    Prepare();
                    Play();
                    break;
            }
            Hooks.Clear();
            OnStateChanged += States.Add;
        }

        protected override void OnPlay(EffectActionState from, Action onCompleted)
        {
            Hooks.Add((Hook.OnPlay, from));
            Completion = onCompleted;
            if (CompleteInstantly)
                onCompleted();
        }

        protected override void OnPrepare(EffectActionState from) => Hooks.Add((Hook.OnPrepare, from));
        protected override void OnStop(EffectActionState from) => Hooks.Add((Hook.OnStop, from));
    }
}
