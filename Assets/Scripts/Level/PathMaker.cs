using System.Collections.Generic;
using System.Linq;
using Donuts;
using Zenject;
using Extensions;

namespace Level
{
    public class PathMaker
    {
        [Inject] private LevelArea _levelArea;
        [Inject] private TransferMaker _transferMaker;

        private Path path;

        public Path GetPath(Cell cell, Cell from)
        {
            path = new();
            Transfer transfer = GetConnection(cell, from);
            if (transfer == null)
            {
                return null;
            }
            if (IsOnlyOneTransferInPath(transfer))
            {
                path.Connection.Enqueue(transfer);
                return path;
            }
            while (PathIsNotFinished(cell, transfer))
            {
                path.Connection.Enqueue(transfer);
                transfer = transfer.AttachedTransfer;
            }
            path.Connection.Reverse();
            return path;
        }


        private Transfer GetConnection(Cell cell, Cell from)
        {
            path.InvolvedCells.Add(cell, from);
            Dictionary<NeighborType, Cell> activeNeighbors = _levelArea.GetCellActiveNeighbors(cell, from);
            SetNeighborsWithSameTopDonut(cell, activeNeighbors);

            if (NeighborsAreCompatible(activeNeighbors) == false && cell.DonutStack.FreeDonutPlaces == 0)
                return null;

            List<Transfer> transfers = GetPossibleTransfers(activeNeighbors, cell);
            transfers = GetTransfersWithHighestPriorities(transfers);

            if (transfers.Count > 1)
            {
                foreach (var t in transfers)
                {
                    t.SimulateTransfer();
                }
                Transfer result = GetSubConnection(transfers, cell);
                if (result == null)
                    return transfers.First();
                return result;
            }
            else
            {
                AddInvolvedCells(path.InvolvedCells, from, activeNeighbors);
                return transfers.First();
            }
        }

        private Transfer GetSubConnection(List<Transfer> highestPriorities, Cell from)
        {
            List<List<Transfer>> highestSubTransfers = new();
            foreach (var h in highestPriorities)
            {
                path.InvolvedCells.AddUnique(h.From, from);
                Dictionary<NeighborType, Cell> activeNeighbors = _levelArea.GetCellActiveNeighbors(h.NeighBor, from);

                AddInvolvedCells(path.InvolvedCells, from, activeNeighbors);
                SetNeighborsWithSimulatedSameTopDonut(from, activeNeighbors);

                List<Transfer> transfers = GetPossibleTransfers(activeNeighbors, from);

                if (NeighborsAreCompatible(activeNeighbors) == false && from.DonutStack.FreeDonutPlaces == 0)
                    continue;

                transfers = GetTransfersWithHighestPriorities(transfers);
                foreach (var t in transfers)
                {
                    t.AttachedTransfer = h;
                    highestSubTransfers.Add(transfers);
                }
            }

            if (highestSubTransfers.Count == 0)
                return null;

            highestSubTransfers = GetTransfersWithHighestPriorities(highestSubTransfers);

            if (highestSubTransfers.Count > 1)
            {
                if (CurrentConnectionPossible(from, highestSubTransfers) == false)
                    return null;

                List<Transfer> allTransfers = GetAllTransfers(highestSubTransfers);
                return GetSubConnection(allTransfers, from);
            }
            else if (highestSubTransfers.Count == 0)
            {
                return highestPriorities.First();
            }
            else
            {
                return highestSubTransfers[0].First();
            }
        }

        private List<Transfer> GetAllTransfers(List<List<Transfer>> highestSubTransfers)
        {
            List<Transfer> allTransfers = new();
            foreach (var d in highestSubTransfers)
            {
                foreach (var v in d)
                {
                    allTransfers.Add(v);
                }
            }
            return allTransfers;
        }

        private bool CurrentConnectionPossible(Cell from, List<List<Transfer>> highestSubTransfers)
        {
            foreach (var l in highestSubTransfers)
            {
                foreach (var p in l)
                {
                    if (p.To != from)
                        return false;
                }
            }
            return true;
        }

        private bool PathIsNotFinished(Cell cell, Transfer transfer)
        {
            return transfer.Cell != cell;
        }

        private bool IsOnlyOneTransferInPath(Transfer transfer)
        {
            return transfer.AttachedTransfer == null;
        }

        private void AddInvolvedCells(Dictionary<Cell, Cell> involvedCells, Cell from, Dictionary<NeighborType, Cell> activeNeighbors)
        {
            foreach (var n in activeNeighbors)
            {
                involvedCells.AddUnique(n.Value, from);
            }
        }

        private List<Transfer> GetPossibleTransfers(Dictionary<NeighborType, Cell> activeNeighbors, Cell cell)
        {
            List<Transfer> transfers = new();
            foreach (var neighbor in activeNeighbors)
            {
                Transfer transfer = _transferMaker.Make(cell, neighbor.Value);
                if (transfer != null)
                    transfers.Add(transfer);
            }
            return transfers;
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

        private void SetNeighborsWithSameTopDonut(Cell cell, Dictionary<NeighborType, Cell> activeNeighbors)
        {
            activeNeighbors = activeNeighbors.Where(
            n => n.Value.DonutStack.GetTopDonut().Type == cell.DonutStack.GetTopDonut().Type).
            ToDictionary(x => x.Key, x => x.Value);
        }

        private void SetNeighborsWithSimulatedSameTopDonut(Cell cell, Dictionary<NeighborType, Cell> activeNeighbors)
        {
            activeNeighbors = activeNeighbors.Where(
            n => n.Value.DonutStack.GetTopSimulatedDonut() == cell.DonutStack.GetTopSimulatedDonut()).
            ToDictionary(x => x.Key, x => x.Value);
        }

        private List<List<Transfer>> GetTransfersWithHighestPriorities(List<List<Transfer>> priorities)
        {
            List<List<Transfer>> result =
            priorities.Where(p => p.First().Priority == Priority.High).ToList();

            if (result.Count == 0)
                result = priorities.Where(p => p.First().Priority == Priority.Medium).ToList();
            if (result.Count == 0)
                result = priorities.Where(p => p.First().Priority == Priority.Low).ToList();
            return result;
        }

        private List<Transfer> GetTransfersWithHighestPriorities(List<Transfer> priorities)
        {
            List<Transfer> result = priorities.Where(p => p.Priority == Priority.High).ToList();
            if (result.Count == 0)
                result = priorities.Where(p => p.Priority == Priority.Medium).ToList();
            if (result.Count == 0)
                result = priorities.Where(p => p.Priority == Priority.Low).ToList();
            return result;
        }
    }
}