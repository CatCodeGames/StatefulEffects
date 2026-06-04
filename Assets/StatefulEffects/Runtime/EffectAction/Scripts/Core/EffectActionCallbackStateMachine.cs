using CatCode.Events;
using System;

namespace CatCode.StatefulEffects
{
    public sealed class EffectActionCallbackStateMachine : IEffectAction
    {
        private readonly static Action<Action> s_emptyCallbackAction = (callback) => callback();
        private readonly static Action s_emptyAction = () => { };

        private readonly EventValue<EffectActionState> _state;
        private readonly bool _ignoreState;

        private readonly Action<Action> _onPlay;
        private readonly Action _onStop;
        private readonly Action _onPrepare;

        private readonly Action _onPlayCompleted;

        private bool _isRunning;
        public bool IsRunning => _isRunning;

        public IReadOnlyEventValue<EffectActionState> State => _state;

        public EffectActionCallbackStateMachine(
            EffectActionState initialState,
            bool ignoreState,
            Action<Action> onPlay,
            Action onPrepare,
            Action onStop)
        {
            _state = new EventValue<EffectActionState>(initialState);
            _ignoreState = ignoreState;
            _onPlay = onPlay ?? s_emptyCallbackAction;
            _onPrepare = onPrepare ?? s_emptyAction;
            _onStop = onStop ?? s_emptyAction;

            _onPlayCompleted = OnPlayCompleted;
        }

        public void Play()
        {
            if (_isRunning && !_ignoreState && _state.Value == EffectActionState.Playing)
                return;
            _isRunning = true;
            _state.Value = EffectActionState.Playing;
            _onPlay(_onPlayCompleted);
        }

        public void Prepare()
        {
            if (_isRunning && !_ignoreState && _state.Value == EffectActionState.Idle)
                return;
            _isRunning = true;
            _state.Value = EffectActionState.Idle;
            _onPrepare();
        }

        public void Stop()
        {
            _onStop();
            _isRunning = false;
        }

        private void OnPlayCompleted() => _state.Value = EffectActionState.Idle;
    }
}
