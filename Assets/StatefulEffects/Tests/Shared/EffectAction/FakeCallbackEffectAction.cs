using System;
using System.Collections.Generic;
using UnityEngine;

namespace CatCode.StatefulEffects.Tests
{
    [AddComponentMenu("")]
    public sealed class FakeCallbackEffectAction : CallbackEffectAction
    {
        public readonly List<(Hook Hook, EffectActionState From)> Hooks = new();

        public int InitializeCount { get; private set; }
        public Action Completion { get; private set; }

        public void SetPolicy(ActionPolicy policy) => Policy = ActionPolicies.Get(policy);

        protected override void OnInitialize() => InitializeCount++;

        protected override void OnPlay(EffectActionState from, Action onCompleted)
        {
            Hooks.Add((Hook.OnPlay, from));
            Completion = onCompleted;
        }

        protected override void OnPrepare(EffectActionState from) => Hooks.Add((Hook.OnPrepare, from));
        protected override void OnStop(EffectActionState from) => Hooks.Add((Hook.OnStop, from));
    }
}
