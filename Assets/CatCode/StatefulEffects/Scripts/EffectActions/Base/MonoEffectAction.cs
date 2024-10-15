using System;
using UnityEngine;

namespace CatCode.EffectActions
{

    public abstract class MonoEffectAction : MonoBehaviour, IEffectAction
    {
        [SerializeField] private EffectActionState _state;
        [SerializeField] private bool _ignoreState;

        public EffectActionState State
        {
            get => _state;
            private set
            {
                if (_state == value)
                    return;
                _state = value;
                StateChanged?.Invoke(_state);
            }
        }

        public event Action<EffectActionState> StateChanged;

        public void Play()
        {
            switch (_state)
            {
                case EffectActionState.Idle:
                    State = EffectActionState.Playing;
                    OnPlay(() => State = EffectActionState.Idle);
                    break;
                case EffectActionState.Playing:
                    if (!_ignoreState)
                        return;
                    OnPlay(() => State = EffectActionState.Idle);
                    break;
            }
        }

        public void Stop()
        {
            OnStop();
            State = EffectActionState.Idle;
        }

        protected abstract void OnPlay(Action callback);
        protected abstract void OnStop();
    }
}