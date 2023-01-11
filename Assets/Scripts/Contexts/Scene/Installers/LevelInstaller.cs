using System.Collections.Generic;
using Level;
using UnityEngine;
using Zenject;

namespace Contexts.Scene.Installers
{
    public class LevelInstaller : MonoInstaller
    {
        [SerializeField] private List<Row> _rows;
        public override void InstallBindings()
        {
            Container.DeclareSignal<SignalRowClicked>();
            Container.DeclareSignal<SignalConnectionFinished>();
            Container.BindInterfacesAndSelfTo<ConnectionHandler>().AsSingle().NonLazy();

            Container.BindInstance(_rows).AsSingle();
            Container.Bind<LevelArea>().AsSingle();
            Container.Bind<PathMaker>().AsSingle();
            Container.BindInterfacesAndSelfTo<ConnectionExecutor>().AsSingle();
        }
    }
}
