using System;

namespace CatCode.StatefulEffects
{
    public abstract class EffectAction : IEffectAction
    {
        private EffectActionState _state;
        private int _currentTransition;

        public EffectActionState State => _state;
        public event Action<EffectActionState> OnStateChanged;

        protected abstract EffectActionPolicy Policy { get; }

        public void Play()
        {
            switch (_state)
            {
                case EffectActionState.None:
                    {
                        Animate(EffectActionState.None);
                        return;
                    }

                case EffectActionState.Ready:
                    {
                        Animate(EffectActionState.Ready);
                        return;
                    }

                case EffectActionState.Playing:
                    {
                        if (Policy.OnRepeat == InterruptResolution.Ignore)
                            return;
                        Animate(EffectActionState.Playing);
                        return;
                    }
            }

            throw new InvalidOperationException($"Unexpected state {_state}.");
        }

        public void Prepare()
        {
            if (_state == EffectActionState.Ready)
                return;

            var transition = ++_currentTransition;
            OnPrepare(_state);

            if (transition == _currentTransition)
                SetState(EffectActionState.Ready);
        }

        public void Stop()
        {
            _currentTransition++;
            OnStop(_state);
            SetState(EffectActionState.None);
        }

        protected abstract void OnPlay(EffectActionState from, Action onCompleted);
        protected abstract void OnPrepare(EffectActionState from);
        protected abstract void OnStop(EffectActionState from);


        private void Animate(EffectActionState fromState)
        {
            var transition = ++_currentTransition;
            SetState(EffectActionState.Playing);

            if (transition != _currentTransition)
                return;

            OnPlay(fromState, () =>
            {
                if (transition == _currentTransition)
                    SetState(EffectActionState.Ready);
            });
        }

        private void SetState(EffectActionState state)
        {
            if (_state == state)
                return;

            _state = state;
            OnStateChanged?.Invoke(_state);
        }
    }
}
