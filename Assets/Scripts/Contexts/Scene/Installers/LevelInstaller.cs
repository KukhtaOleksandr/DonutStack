using Level;
using Zenject;

namespace Contexts.Scene.Installers
{
    public class LevelInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.DeclareSignal<SignalRowClicked>();
            Container.DeclareSignal<SignalConnectionFinished>();
            Container.BindInterfacesAndSelfTo<ConnectionHandler>().AsSingle().NonLazy();

        }
    }
}
