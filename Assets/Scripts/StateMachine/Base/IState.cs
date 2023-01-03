namespace StateMachine.Base
{
    public interface IState
    {
        void Enter();
        void Exit();
    }
}