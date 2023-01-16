using Donuts;
using Level;
using StateMachine.Base;
using Zenject;

namespace Architecture.StateMachine
{
    public class InConnectionState : IState
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private DonutFactory _donutFactory;
        public void Enter()
        {
            _signalBus.Subscribe<SignalConnectionFinished>(OnConnectionFinished);
        }

        public void Exit()
        {
            _signalBus.Unsubscribe<SignalConnectionFinished>(OnConnectionFinished);
        }

        private void OnConnectionFinished()
        {
            _donutFactory.Create();
            _signalBus.Fire<SignalChangedState>(new SignalChangedState() { State = new HandleRowClickState() });
        }
    }
}