using Input;
using Zenject;

namespace Contexts.Scene.Installers
{
    public class InputInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.DeclareSignal<SignalMouseClicked>();
        }
    }
}