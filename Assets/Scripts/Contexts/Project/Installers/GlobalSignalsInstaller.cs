using System.ComponentModel;
using Zenject;

namespace Contexts.Project.Installers
{
    public class GlobalSignalsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
        }
    }
}