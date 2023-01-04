using Zenject;

namespace Level
{
    public class RowsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.DeclareSignal<SignalRowClicked>();
        }
    }
}