using System;
using System.Collections.Generic;

namespace CatCode.Common
{
    public sealed class StateTransition<TState> where TState : Enum
    {
        private readonly EqualityComparer<TState> _comparer;
        private readonly EventValue<TState> _state;
        
        private TState StateValue
        {
            get => _state.Value;
            set => _state.Value = value;
        }

        public IReadOnlyEventValue<TState> State => _state;

        public bool IsActive { get; private set; }

        public StateTransition(TState initial)
            : this(initial, EqualityComparer<TState>.Default) { }

        public StateTransition(TState initial, EqualityComparer<TState> comparer)
        {
            _comparer = comparer;
            _state = new EventValue<TState>(initial);
        }

        public void Run(TState via, TState to, Action<Action> execute, bool ignoreState = false)
        {
            if (!ignoreState && _comparer.Equals(StateValue, via) || _comparer.Equals(StateValue, to))
                return;

            BeginTransition(via, to, execute);
        }

        public void RunForce(TState via, TState to, Action<Action> execute)
            => BeginTransition(via, to, execute);


        public void BeginTransition(TState via, TState to, Action<Action> execute)
        {
            IsActive = true;
            StateValue = via;

            execute(() =>
            {
                StateValue = to;
                IsActive = false;
            });
        }

        public void Set(TState state, Action action)
        {
            IsActive = true;
            action();
            StateValue = state;
            IsActive = false;
        }

        public void Stop()
        {
            IsActive = false;
        }
    }
}