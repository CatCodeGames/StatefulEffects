using System;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    public abstract class CallbackShowHide : MonoShowHide
    {
        [SerializeField] private ShowHidePolicy _policy;

        private ShowHideCore _core;

        public override ShowHideState State => _core == null 
            ? ShowHideState.None 
            : _core.State;

        public override event Action<ShowHideState> OnStateChanged;

        protected ShowHidePolicy Policy
        {
            get => _policy;
            set => _policy = value;
        }

        private ShowHideCore Core
        {
            get
            {
                if (_core == null)
                {
                    _core = new ShowHideCore(this);
                    _core.OnStateChanged += state => OnStateChanged?.Invoke(state);
                    OnInitialize();
                }
                return _core;
            }
        }

        protected virtual void OnDisable()
        {
            _core?.Stop();
        }

        protected virtual void OnDestroy()
        {
            if (_core == null)
                return;

            switch (_core.State)
            {
                case ShowHideState.None:
                    break;
                case ShowHideState.Hidden:
                    _core.Stop();
                    break;
                case ShowHideState.Showing:
                    _core.Stop();
                    break;
                case ShowHideState.Shown:
                    _core.Stop();
                    break;
                case ShowHideState.Hiding:
                    _core.Stop();
                    break;
            }
        }

        public override void Show() => Core.Show();
        public override void Hide() => Core.Hide();
        public override void SetShown() => Core.SetShown();
        public override void SetHidden() => Core.SetHidden();
        public override void Stop() => Core.Stop();

        protected virtual void OnInitialize() { }
        protected abstract void OnShow(ShowHideState from, Action onCompleted);
        protected abstract void OnHide(ShowHideState from, Action onCompleted);
        protected abstract void OnSetShown(ShowHideState from);
        protected abstract void OnSetHidden(ShowHideState from);
        protected abstract void OnStop(ShowHideState from);


        private sealed class ShowHideCore : ShowHide
        {
            private readonly CallbackShowHide _owner;

            public ShowHideCore(CallbackShowHide owner)
                => _owner = owner;

            protected override ShowHidePolicy Policy => _owner._policy;

            protected override void OnShow(ShowHideState from, Action onCompleted) => _owner.OnShow(from, onCompleted);
            protected override void OnHide(ShowHideState from, Action onCompleted) => _owner.OnHide(from, onCompleted);
            protected override void OnSetShown(ShowHideState from) => _owner.OnSetShown(from);
            protected override void OnSetHidden(ShowHideState from) => _owner.OnSetHidden(from);
            protected override void OnStop(ShowHideState from) => _owner.OnStop(from);
        }
    }
}
