using System;
using System.Collections.Generic;
using Donuts;
using Donuts.StateMachine;
using Zenject;

namespace Level
{
    public class ConnectionHandler : IInitializable, IDisposable
    {
        [Inject] SignalBus _signalBus;
        public Row CurrentRow { get; set; }
        public Cell CurrentCell { get; set; }

        public void Initialize()
        {
            _signalBus.Subscribe<SignalDonutMovedToPosition>(GetPath);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<SignalDonutMovedToPosition>(GetPath);
        }

        private async void GetPath()
        {
            Dictionary<NeighborType, Cell> neighbors = LevelArea.GetCellActiveNeighbors(CurrentRow, CurrentCell);

            if (neighbors.TryGetValue(NeighborType.Top, out Cell cell))
            {
                List<Donut> donuts = new List<Donut>(CurrentCell.DonutStack.Donuts);
                foreach (Donut donut in donuts)
                {
                    if (DonutsTypesEqual(cell))
                    {
                        await CurrentCell.DonutStack.TransferTopDonutToOther(cell.DonutStack);
                    }
                }

            }
            _signalBus.Fire<SignalConnectionFinished>();
        }

        private bool DonutsTypesEqual(Cell cell)
        {
            return cell.DonutStack.GetTopDonut().Type == CurrentCell.DonutStack.GetTopDonut().Type;
        }
    }
}