using System;
using Zenject;

namespace StateMachine.Base
{
    public abstract class StateMachineBase : IInitializable, IDisposable
    {
        private IState currentState;

        private readonly DiContainer _container;
        private readonly SignalBus _signalBus;

        public StateMachineBase(DiContainer container, SignalBus signalBus)
        {
            _container = container;
            _signalBus = signalBus;
        }

        void IInitializable.Initialize()
        {
            _signalBus.Subscribe<SignalChangedState>(OnChangedState);
            Initialize();
        }

        void IDisposable.Dispose()
        {
            _signalBus.Unsubscribe<SignalChangedState>(OnChangedState);
        }

        private void OnChangedState(SignalChangedState args)
        {
            ChangeState(args.State.GetType());
        }

        protected void ChangeState<TState>() where TState : IState
        {
            ChangeState(typeof(TState));
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