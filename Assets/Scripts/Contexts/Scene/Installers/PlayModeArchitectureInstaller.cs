using Architecture.StateMachine;
using StateMachine.Base;
using UnityEngine;
using Zenject;

namespace Contexts.Scene.Installers
{
    public class PlayModeArchitectureInstaller : MonoInstaller
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _layerMask;
        public override void InstallBindings()
        {
            Container.DeclareSignal<SignalChangedState>();
            Container.BindInstance(_camera).WhenInjectedInto<HandleRowClickState>();
            Container.BindInstance(_layerMask).WhenInjectedInto<HandleRowClickState>();
            Container.BindInterfacesTo<PlayStateMachine>().AsSingle().NonLazy();
        }
    }
}