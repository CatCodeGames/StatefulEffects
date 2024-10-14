using System.Collections.Generic;
using System;
using UnityEngine;

namespace CatCode
{
    [Serializable]
    public sealed class StateTransitionController<TState> where TState : Enum
    {
        private Action<Action> _transitionAction;
        private readonly EqualityComparer<TState> _stateComparer;
        private bool _isChanging = false;
        private Action _callback = null;

        [SerializeField] private TState _state;
        [SerializeField] private bool _ignoreState;

        public event Action<TState> StateChanged;


        public bool IsChanging
        {
            get => _isChanging;
            private set => _isChanging = value;
        }

        public TState State
        {
            get => _state;
            private set
            {

                if (_stateComparer.Equals(_state, value))
                    return;
                _state = value;
                StateChanged?.Invoke(_state);
            }
        }

        public StateTransitionController() : this(EqualityComparer<TState>.Default)
        { }

        public StateTransitionController(EqualityComparer<TState> stateComparer)
        {
            _stateComparer = stateComparer;
        }

        public StateTransitionController(EqualityComparer<TState> stateComparer, TState state, bool ignoreState)
        {
            _stateComparer = stateComparer;
            _state = state;
            _ignoreState = ignoreState;
        }

        public void TransitionToState(Action<Action> transitionAction, in TState intermediateState, in TState targetState, Action callback = null)
        {
            _callback = callback;
            if (_ignoreState)
            {
                ToInvoke(transitionAction, intermediateState, targetState);
                return;
            }
            if (_stateComparer.Equals(_state, targetState))
            {
                _callback?.Invoke();
                return;
            }
            if (_stateComparer.Equals(_state, intermediateState))
                return;

            ToInvoke(transitionAction, intermediateState, targetState);
        }

        public void SetState(in TState targetState)
        {
            IsChanging = false;
            State = targetState;
        }

        public void Stop()
        {
            _transitionAction = null;
            _isChanging = false;
        }

        private void ToInvoke(Action<Action> transitionAction, in TState intermediateState, TState targetState)
        {
            IsChanging = true;
            State = intermediateState;
            _transitionAction = transitionAction;
            _transitionAction(() =>
            {
                IsChanging = false;
                State = targetState;
                _callback?.Invoke();
            });
        }
    }
}