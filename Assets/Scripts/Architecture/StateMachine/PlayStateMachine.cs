using StateMachine.Base;
using Zenject;

namespace Architecture.StateMachine
{
    public class PlayStateMachine : StateMachineBase
    {
        [Inject] private DiContainer _container;
        [Inject] private SignalBus _signalBus;
        public PlayStateMachine(DiContainer container, SignalBus signalBus) : base(container, signalBus)
        {

        }

        protected override void Initialize()
        {
            ChangeState<LoadPlayModeState>();
        }
    }
}