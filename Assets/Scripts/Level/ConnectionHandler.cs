using System;
using System.Collections.Generic;
using System.Linq;
using Donuts.StateMachine;
using Zenject;

namespace Level
{
    public class ConnectionHandler : IInitializable, IDisposable
    {
        private const float Offset = 0.3f;

        [Inject] SignalBus _signalBus;
        [Inject] LevelArea _levelArea;
        [Inject] PathMaker _pathMaker;
        public Row CurrentRow { get; set; }
        public Cell CurrentCell { get; set; }

        public void Initialize()
        {
            _signalBus.Subscribe<SignalDonutMovedToPosition>(MakeConnection);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<SignalDonutMovedToPosition>(MakeConnection);
        }

        private async void MakeConnection()
        {
            Dictionary<NeighborType, Cell> neighbors = GetNeighborsWithSameTopDonut(CurrentCell, null);

            if (neighbors.Count == 0)
            {
                _signalBus.Fire<SignalConnectionFinished>();
                return;
            }

            Path path = _pathMaker.GetPath(CurrentCell, null);
            if (PathExists(path))
            {
                bool isFirstTransfer = false;
                while (path.Connection.Count > 0)
                {
                    Transfer transfer = path.Connection.Dequeue();
                    if (isFirstTransfer == false)
                    {
                        path.InvolvedCells[transfer.Cell] = transfer.NeighBor;
                        isFirstTransfer = true;
                    }

                    while (transfer.From.CanTransferDonutTo(transfer.To))
                        await transfer.TransferDonut();
                }
                _levelArea.ResetSimulatedDonuts();

                while (InvolvedCellsExist(path))
                {
                    var cell = path.InvolvedCells.First();
                    if (DonutStackExists(cell))
                    {
                        if (CellCanHaveConnection(cell))
                        {
                            Dictionary<NeighborType, Cell> subNeighbors = GetNeighborsWithSameTopDonut(cell.Key, cell.Value);
                            if (subNeighbors.Count == 0)
                            {
                                path.InvolvedCells.Remove(cell.Key);
                                continue;
                            }

                            if (NeighborsAreCompatible(subNeighbors) || cell.Key.DonutStack.FreeDonutPlaces > 0)
                            {
                                Path subPath = _pathMaker.GetPath(cell.Key, cell.Value);
                                if (PathExists(subPath))
                                {
                                    path.InvolvedCells = MergePathsInvolvedCells(path, subPath);
                                    while (subPath.Connection.Count > 0)
                                    {
                                        Transfer transfer = subPath.Connection.Dequeue();
                                        while (transfer.From.CanTransferDonutTo(transfer.To))
                                            await transfer.TransferDonut();
                                    }
                                }
                                _levelArea.ResetSimulatedDonuts();
                            }
                        }
                    }
                    path.InvolvedCells.Remove(cell.Key);

                    if (path.InvolvedCells.Count == 0)
                        break;

                    if (DonutStackExists(cell))
                    {
                        if (CheckForConnectionFinish(cell,path))
                        {
                            break;
                        }
                    }
                }
            }

            _signalBus.Fire<SignalConnectionFinished>();
        }

        private bool CheckForConnectionFinish(KeyValuePair<Cell, Cell> cell, Path path)
        {
            if (cell.Key.DonutStack.FreeDonutPlaces < 3 && cell.Key.DonutStack.IsFull() == false)
            {
                try
                {
                    path.InvolvedCells.Add(cell.Key, cell.Value);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        private Dictionary<Cell, Cell> MergePathsInvolvedCells(Path path, Path subPath)
        {
            Dictionary<Cell, Cell> result = path.InvolvedCells.Concat(subPath.InvolvedCells.Where(x => !path.InvolvedCells.Keys.
             Contains(x.Key))).ToDictionary(x => x.Key, x => x.Value);
            return result;
        }

        private bool NeighborsAreCompatible(Dictionary<NeighborType, Cell> activeNeighbors)
        {
            bool isNeighborWithFreeDonutPlaces = false;
            foreach (var n in activeNeighbors)
            {
                if (n.Value.DonutStack.FreeDonutPlaces > 0)
                {
                    isNeighborWithFreeDonutPlaces = true;
                    break;
                }
            }
            return isNeighborWithFreeDonutPlaces;
        }

        private bool CellCanHaveConnection(KeyValuePair<Cell, Cell> cell)
        {
            return cell.Key.DonutStack.FreeDonutPlaces < 3 && cell.Key.DonutStack.IsFull() == false;
        }

        private bool DonutStackExists(KeyValuePair<Cell, Cell> cell)
        {
            return cell.Key.DonutStack != null;
        }

        private bool InvolvedCellsExist(Path path)
        {
            return path.InvolvedCells.Count > 0;
        }

        private bool PathExists(Path path)
        {
            return path != null;
        }

        private Dictionary<NeighborType, Cell> GetNeighborsWithSameTopDonut(Cell cell, Cell from)
        {
            return _levelArea.GetCellActiveNeighbors(cell, from).
            Where(n => n.Value.DonutStack.GetTopDonut().Type == cell.DonutStack.GetTopDonut().Type).
            ToDictionary(x => x.Key, x => x.Value);
        }

        private bool DonutsTypesEqual(Cell cell)
        {
            return cell.DonutStack.GetTopDonut().Type == CurrentCell.DonutStack.GetTopDonut().Type;
        }
    }
}