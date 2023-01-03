using UnityEngine;
using Zenject;
using System;

namespace StateMachine.Base
{
    public abstract class MonoStateMachineBase : MonoBehaviour
    {
        private IState currentState;

        [Inject]
        private readonly DiContainer _container;
        [Inject]
        private readonly SignalBus _signalBus;

        void OnEnable()
        {
            _signalBus.Subscribe<MonoSignalChangedState>(OnChangedState);
            Initialize();
        }

        void OnDisable()
        {
            _signalBus.Unsubscribe<MonoSignalChangedState>(OnChangedState);
        }

        private void OnChangedState(MonoSignalChangedState args)
        {
            //ChangeState(args.State.GetType());
            ChangeState(args.State);
        }

        protected void ChangeState<TState>() where TState : IState
        {
            ChangeState(typeof(TState));
            //ChangeState(typeof(TState));
        }

        protected void ChangeState(IState state)
        {
            currentState?.Exit();
            currentState = state;
            currentState.Enter();
        }
        protected void ChangeState(Type state)
        {
            currentState?.Exit();
            currentState = CreateState(state);
            currentState.Enter();
        }

        private IState CreateState(Type state)
        {
            return _container.Instantiate(state) as IState;
        }

        protected abstract void Initialize();
    }

}