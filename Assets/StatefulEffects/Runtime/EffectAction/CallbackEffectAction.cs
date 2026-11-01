using System;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    public abstract class CallbackEffectAction : MonoEffectAction
    {
        [SerializeField] private EffectActionPolicy _policy;

        private EffectActionCore _core;

        public override EffectActionState State => _core == null
            ? EffectActionState.None
            : _core.State;

        public override event Action<EffectActionState> OnStateChanged;

        protected EffectActionPolicy Policy
        {
            get => _policy;
            set => _policy = value;
        }

        private EffectActionCore Core
        {
            get
            {
                if (_core == null)
                {
                    _core = new EffectActionCore(this);
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
                case EffectActionState.None:
                    break;
                case EffectActionState.Ready:
                    _core.Stop();
                    break;
                case EffectActionState.Playing:
                    _core.Stop();
                    break;
            }
        }

        public override void Play() => Core.Play();
        public override void Prepare() => Core.Prepare();
        public override void Stop() => Core.Stop();

        protected virtual void OnInitialize() { }
        protected abstract void OnPlay(EffectActionState from, Action onCompleted);
        protected abstract void OnPrepare(EffectActionState from);
        protected abstract void OnStop(EffectActionState from);


        private sealed class EffectActionCore : EffectAction
        {
            private readonly CallbackEffectAction _owner;

            public EffectActionCore(CallbackEffectAction owner)
                => _owner = owner;

            protected override EffectActionPolicy Policy => _owner._policy;

            protected override void OnPlay(EffectActionState from, Action onCompleted) => _owner.OnPlay(from, onCompleted);
            protected override void OnPrepare(EffectActionState from) => _owner.OnPrepare(from);
            protected override void OnStop(EffectActionState from) => _owner.OnStop(from);
        }
    }
}
