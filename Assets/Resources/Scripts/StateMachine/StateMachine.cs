using System;
using System.Collections.Generic;

namespace Platformer {
    public class StateMachine {
        private StateNode current;
        private Dictionary<Type, StateNode> nodes = new Dictionary<Type, StateNode>();
        private HashSet<ITransition> anyTransitions = new HashSet<ITransition>();
        private class StateNode {
            public IState State { get; }
            public HashSet<ITransition> Transitions { get; }
            
            public StateNode(IState state) {
                State = state;
                Transitions = new HashSet<ITransition>();
            }

            public void AddTransition(IState to, IPredicate condition) {
                Transitions.Add(new Transition(to, condition));
            }
        }

        public void Update() {
            ITransition transition = GetTransition();
            if (transition != null) {
                ChangeState(transition.To);
            }
            
            current.State?.OnUpdate();
        }

        public void FixedUpdate() {
            current.State?.OnFixedUpdate();
        }

        public void SetState(IState state) {
            current = nodes[state.GetType()];
            current.State?.OnEnter();
        }

        private void ChangeState(IState transitionTo) {
            if (current.State == transitionTo) return;
            
            IState previousState = current.State;
            IState nextState = nodes[transitionTo.GetType()].State;;
            
            previousState?.OnExit();
            nextState?.OnEnter();
            
            current = nodes[transitionTo.GetType()];
        }
        
        public void AddTransition(IState from, IState to, IPredicate condition) {
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
        }

        public void AddAnyTransition(IState to, IPredicate condition) {
            anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));       
        }
        
        private StateNode GetOrAddNode(IState state) {
            StateNode node = nodes.GetValueOrDefault(state.GetType());
            
            if (node == null) {
                node = new StateNode(state);
                nodes.Add(state.GetType(), node);
            }
            
            return node;       
        }

        private ITransition GetTransition() {
            foreach (ITransition transition in anyTransitions) 
                if (transition.Condition.Evaluate()) 
                    return transition;

            foreach (ITransition transition in current.Transitions) 
                if (transition.Condition.Evaluate()) 
                    return transition;   
            
            return null;
        }
    }
}