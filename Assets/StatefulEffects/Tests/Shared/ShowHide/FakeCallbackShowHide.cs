using System;
using System.Collections.Generic;
using UnityEngine;

namespace CatCode.StatefulEffects.Tests
{
    [AddComponentMenu("")]
    public sealed class FakeCallbackShowHide : CallbackShowHide
    {
        public readonly List<(Hook Hook, ShowHideState From)> Hooks = new();

        public int InitializeCount { get; private set; }
        public Action Completion { get; private set; }

        public void SetPolicy(Policy policy) => Policy = Policies.Get(policy);

        protected override void OnInitialize() => InitializeCount++;

        protected override void OnShow(ShowHideState from, Action onCompleted)
        {
            Hooks.Add((Hook.OnShow, from));
            Completion = onCompleted;
        }

        protected override void OnHide(ShowHideState from, Action onCompleted)
        {
            Hooks.Add((Hook.OnHide, from));
            Completion = onCompleted;
        }

        protected override void OnSetShown(ShowHideState from) => Hooks.Add((Hook.OnSetShown, from));
        protected override void OnSetHidden(ShowHideState from) => Hooks.Add((Hook.OnSetHidden, from));
        protected override void OnStop(ShowHideState from) => Hooks.Add((Hook.OnStop, from));
    }
}
