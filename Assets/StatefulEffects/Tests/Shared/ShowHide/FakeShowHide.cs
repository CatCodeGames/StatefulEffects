using System;
using System.Collections.Generic;

namespace CatCode.StatefulEffects.Tests
{
    public sealed class FakeShowHide : ShowHide
    {
        public readonly List<(Hook Hook, ShowHideState From)> Hooks = new();
        public readonly List<ShowHideState> States = new();

        private readonly ShowHidePolicy _policy;

        public Action Completion { get; private set; }
        public bool CompleteInstantly { get; set; }

        protected override ShowHidePolicy Policy => _policy;

        public FakeShowHide(ShowHideState state, Policy policy = default)
        {
            _policy = Policies.Get(policy);
            switch (state)
            {
                case ShowHideState.None:
                    break;
                case ShowHideState.Hidden:
                    SetHidden();
                    break;
                case ShowHideState.Showing:
                    SetHidden();
                    Show();
                    break;
                case ShowHideState.Shown:
                    SetShown();
                    break;
                case ShowHideState.Hiding:
                    SetShown();
                    Hide();
                    break;
            }
            Hooks.Clear();
            OnStateChanged += States.Add;
        }

        protected override void OnShow(ShowHideState from, Action onCompleted)
        {
            Hooks.Add((Hook.OnShow, from));
            Completion = onCompleted;
            if (CompleteInstantly)
                onCompleted();
        }

        protected override void OnHide(ShowHideState from, Action onCompleted)
        {
            Hooks.Add((Hook.OnHide, from));
            Completion = onCompleted;
            if (CompleteInstantly)
                onCompleted();
        }

        protected override void OnSetShown(ShowHideState from) => Hooks.Add((Hook.OnSetShown, from));
        protected override void OnSetHidden(ShowHideState from) => Hooks.Add((Hook.OnSetHidden, from));
        protected override void OnStop(ShowHideState from) => Hooks.Add((Hook.OnStop, from));
    }
}
