using Zenject;
using UnityEngine;
using Donuts;

namespace Contexts.Scene.Installers
{
    public class DonutInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _donutPrefab;
        [SerializeField] private Transform _spawnPosition;

        public override void InstallBindings()
        {
            Container.BindInstance(_donutPrefab).AsSingle().WhenInjectedInto<DonutFactory>();
            Container.BindInstance(_spawnPosition).AsSingle().WhenInjectedInto<DonutFactory>();
            Container.Bind<DonutFactory>().AsSingle();
        }
    }
}