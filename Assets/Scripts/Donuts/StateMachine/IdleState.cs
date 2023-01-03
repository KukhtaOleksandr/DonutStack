using Rows;
using StateMachine.Base;
using Zenject;

namespace Donuts.StateMachine
{
    public class IdleState : IState
    {
        [Inject]
        private SignalBus _signalBus;
        [Inject]
        private DiContainer _container;
        public void Enter()
        {
            _signalBus.Subscribe<SignalRowClicked>(ChangeState);
        }

        public void Exit()
        {
            _signalBus.Unsubscribe<SignalRowClicked>(ChangeState);
        }

        private void ChangeState(SignalRowClicked args)
        {
            MoveToPositionState moveToPositionState = _container.Instantiate<MoveToPositionState>();
            moveToPositionState.PositionToMove= args.Position;
            _signalBus.Fire<MonoSignalChangedState>(new MonoSignalChangedState() 
            { State = moveToPositionState });

        }
    }
}