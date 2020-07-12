using System;
using System.Collections;
using System.Collections.Generic;

namespace RedOwl.Core
{
    public interface IState
    {
        
    }

    public interface IStateEnterable
    {
        void OnEnter();
    }

    public interface IStateExecute
    {
        IEnumerator OnExecute();
    }
    
    public interface IStateExitable
    {
        void OnExit();
    }
    
    public class State
    {
        private IState _state;
        private ITransition[] _transitions;

        public State(IState state, ITransition[] transitions)
        {
            _state = state;
            _transitions = transitions;
        }

        internal void Inject()
        {
            Game.Inject(_state);
        }

        internal void Enter()
        {
            foreach (var transition in _transitions)
            {
                transition.Enable();
            }

            if (_state is IStateEnterable e) e.OnEnter();
        }

        internal IEnumerator Execute()
        {
            if (_state is IStateExecute e) yield return e.OnExecute();
        }

        internal void Exit()
        {
            if (_state is IStateExitable e) e.OnExit();
            foreach (var transition in _transitions)
            {
                transition.Disable();
            }
        }
    }

    public class NullState : IState
    {
        public IEnumerator OnExecute()
        {
            yield break;
        }
    }

    public class StateBuilder
    {
        public Guid Id { get; private set; }
        private IState _state;
        private List<TransitionBuilder> _transitions;

        public StateBuilder()
        {
            Id = Guid.NewGuid();
            _transitions = new List<TransitionBuilder>();
        }
        
        public void WithState(IState state)
        {
            _state = state;
        }
        
        public void Permit(IMessage message, StateBuilder state, Func<bool> guard = null)
        {
            _transitions.Add(guard == null
                ? new TransitionBuilder(message, state.Id)
                : new TransitionBuilder(message, state.Id, guard));
        }

        public State Build(StateMachine machine)
        {
            var transitions = new List<ITransition>();
            foreach (var transition in _transitions)
            {
                transitions.Add(transition.Build(machine));
            }
            return new State(_state, transitions.ToArray());
        }


    }
}