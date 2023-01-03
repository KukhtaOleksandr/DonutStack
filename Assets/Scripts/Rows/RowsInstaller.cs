using Zenject;

namespace Rows
{
    public class RowsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.DeclareSignal<SignalRowClicked>();
        }
    }
}