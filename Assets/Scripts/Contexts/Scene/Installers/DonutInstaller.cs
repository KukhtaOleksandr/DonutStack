using Zenject;
using UnityEngine;
using Donuts;
using Donuts.StateMachine;
using System.Collections.Generic;

namespace Contexts.Scene.Installers
{
    public class DonutInstaller : MonoInstaller
    {
        [SerializeField] private List<DonutStack> _donutPrefabs;
        [SerializeField] private Transform _spawnPosition;

        public override void InstallBindings()
        {
            Container.DeclareSignal<SignalDonutMovedToPosition>();
            Container.BindInstance(_donutPrefabs).AsSingle().WhenInjectedInto<DonutFactory>();
            Container.BindInstance(_spawnPosition).AsSingle().WhenInjectedInto<DonutFactory>();
            Container.BindInterfacesAndSelfTo<DonutFactory>().AsSingle().NonLazy();
            Container.Bind<DonutsChangeHandler>().AsSingle();
        }
    }
}