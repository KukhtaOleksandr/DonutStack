using Zenject;
using UnityEngine;
using Donuts;
using Donuts.StateMachine;

namespace Contexts.Scene.Installers
{
    public class DonutInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _donutPrefab;
        [SerializeField] private Transform _spawnPosition;

        public override void InstallBindings()
        {
            Container.DeclareSignal<SignalDonutMovedToPosition>();
            Container.BindInstance(_donutPrefab).AsSingle().WhenInjectedInto<DonutFactory>();
            Container.BindInstance(_spawnPosition).AsSingle().WhenInjectedInto<DonutFactory>();
            Container.BindInterfacesTo<DonutFactory>().AsSingle().NonLazy();
        }
    }
}