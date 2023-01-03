using Donuts.StateMachine;
using StateMachine.Base;
using Zenject;

namespace Donuts
{
    public class DonutInstaller : MonoInstaller
    {
        
        public override void InstallBindings()
        {
            Container.DeclareSignal<MonoSignalChangedState>();
            Container.BindInstance(this.transform).AsSingle().WhenInjectedInto<MoveToPositionState>();
        }
    }
}