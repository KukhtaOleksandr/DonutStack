using System.IO;
using System.Collections.Generic;
using System.Linq;
using Donuts;
using Zenject;

namespace Level
{
    public class PathMaker
    {
        [Inject] private LevelArea _levelArea;

        private Path path;

        public Path GetPath(Cell cell, Cell from)
        {
            path = new();
            Transfer transfer = GetConnection(cell, from);
            if (transfer == null)
            {
                return null;
            }
            if (transfer.AttachedTransfer == null)
            {
                path.Connection.Enqueue(transfer);
                return path;
            }

            while (transfer.Cell != cell)
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

            Dictionary<NeighborType, Cell> activeNeighborsWithSameTop = activeNeighbors.Where(
            n => n.Value.DonutStack.GetTopDonut().Type == cell.DonutStack.GetTopDonut().Type).
            ToDictionary(x => x.Key, x => x.Value);

            bool isNeighborWithFreeDonutPlaces = false;
            foreach (var n in activeNeighborsWithSameTop)
            {
                if (n.Value.DonutStack.FreeDonutPlaces > 0)
                {
                    isNeighborWithFreeDonutPlaces = true;
                    break;
                }
            }

            if (isNeighborWithFreeDonutPlaces == false && cell.DonutStack.FreeDonutPlaces == 0)
                return null;

            List<Transfer> priorities = new();

            foreach (var neighbor in activeNeighborsWithSameTop)
            {
                Transfer transfer = GetTransferPriority(cell, neighbor.Value);
                priorities.Add(transfer);
            }

            List<Transfer> highestPriorities = GetHighestPriorities(priorities);
            if (highestPriorities.Count > 1)
            {
                foreach (var t in highestPriorities)
                {
                    t.SimulateTransfer();
                }
                Transfer result = GetSubConnection(highestPriorities, cell);
                if (result == null)
                    return highestPriorities.First();
                return result;
            }
            else
            {
                foreach (var n in activeNeighbors)
                {
                    path.InvolvedCells.AddUnique(n.Value, cell);
                }
                return highestPriorities.First();
            }
        }

        private Transfer GetSubConnection(List<Transfer> highestPriorities, Cell from)
        {
            List<List<Transfer>> highestSubPriorities = new();
            foreach (var h in highestPriorities)
            {
                path.InvolvedCells.AddUnique(h.From, from);
                Dictionary<NeighborType, Cell> activeNeighbors = _levelArea.GetCellActiveNeighbors(h.NeighBor, from);
                foreach (var n in activeNeighbors)
                {
                    path.InvolvedCells.AddUnique(n.Value, from);
                }
                Dictionary<NeighborType, Cell> activeNeighborsWithSameTop = activeNeighbors.Where(
                    n => n.Value.DonutStack.GetTopSimulatedDonut() == h.Cell.DonutStack.GetTopSimulatedDonut()).ToDictionary(x => x.Key, x => x.Value);

                bool isNeighborWithFreeDonutPlaces = false;
                foreach (var n in activeNeighborsWithSameTop)
                {
                    if (n.Value.DonutStack.FreeDonutPlaces > 0)
                    {
                        isNeighborWithFreeDonutPlaces = true;
                        break;
                    }
                }

                List<Transfer> priorities = new();

                foreach (var neighbor in activeNeighborsWithSameTop)
                {
                    Transfer transfer = GetTransferPriority(h.NeighBor, neighbor.Value);
                    if (transfer != null)
                        priorities.Add(transfer);
                }

                if (isNeighborWithFreeDonutPlaces == false && priorities.Count == 0)
                    continue;

                List<Transfer> localPriorities = GetHighestPriorities(priorities);
                foreach (var p in localPriorities)
                {
                    p.AttachedTransfer = h;
                    highestSubPriorities.Add(localPriorities);
                }
            }
            if (highestSubPriorities.Count == 0)
                return null;
            highestSubPriorities = GetHighestPriorities(highestSubPriorities);
            if (highestSubPriorities.Count > 1)
            {
                bool returnFirst = true;
                foreach (var l in highestSubPriorities)
                {
                    foreach (var p in l)
                    {
                        if (p.To != from)
                            returnFirst = false;
                    }
                }
                if (returnFirst)
                    return null;

                List<Transfer> newPriorities = new();
                foreach (var d in highestSubPriorities)
                {
                    foreach (var v in d)
                    {
                        newPriorities.Add(v);
                    }
                }
                return GetSubConnection(newPriorities, from);
            }
            else if (highestSubPriorities.Count == 0)
            {
                return highestPriorities.First();
            }
            else
            {
                return highestSubPriorities[0].First();
            }
        }

        private List<List<Transfer>> GetHighestPriorities(List<List<Transfer>> priorities)
        {
            List<List<Transfer>> result =
            priorities.Where(p => p.First().Priority == Priority.High).ToList();

            if (result.Count == 0)
                result = priorities.Where(p => p.First().Priority == Priority.Medium).ToList();
            if (result.Count == 0)
                result = priorities.Where(p => p.First().Priority == Priority.Low).ToList();
            return result;
        }

        private List<Transfer> GetHighestPriorities(List<Transfer> priorities)
        {
            List<Transfer> result = priorities.Where(p => p.Priority == Priority.High).ToList();
            if (result.Count == 0)
                result = priorities.Where(p => p.Priority == Priority.Medium).ToList();
            if (result.Count == 0)
                result = priorities.Where(p => p.Priority == Priority.Low).ToList();
            return result;
        }

        private Transfer GetTransferPriority(Cell cell, Cell neighborCell)
        {
            Transfer transfer = new Transfer();
            DonutStack stack = cell.DonutStack;
            DonutStack neighborStack = neighborCell.DonutStack;

            List<DonutType> donuts1 = new();
            foreach (Donut d in cell.DonutStack.GetTopDonutsOfOneType())
            {
                donuts1.Add(d.Type);
            }
            List<DonutType> donuts2 = neighborCell.DonutStack.GetTopSimulatedDonutsOfOneType();

            if ((stack.FreeDonutPlaces > 0 || neighborStack.FreeDonutPlaces > 0))
            {
                transfer.Cell = cell;
                transfer.NeighBor = neighborCell;
                if (AreOtherTypesInStack(cell, donuts1) && AreOtherTypesInStack(neighborCell, donuts2))
                {
                    transfer.Priority = Priority.Low;
                    if (donuts1.Count > donuts2.Count)
                    {
                        transfer.From = cell; transfer.To = neighborCell;
                    }
                    else if (donuts1.Count < donuts2.Count)
                    {
                        transfer.From = neighborCell; transfer.To = cell;
                    }
                    else
                    {
                        if (transfer.Cell.DonutStack.FreeDonutPlaces > transfer.NeighBor.DonutStack.FreeDonutPlaces)
                        {
                            transfer.From = neighborCell; transfer.To = cell;
                        }
                        else if (transfer.Cell.DonutStack.FreeDonutPlaces < transfer.NeighBor.DonutStack.FreeDonutPlaces)
                        {
                            transfer.From = cell; transfer.To = neighborCell;
                        }
                        else
                        {
                            //TODO: return best transfer
                            transfer.From = neighborCell; transfer.To = cell;
                        }
                    }
                    return transfer;
                }
                if (AreOtherTypesInStack(cell, donuts1) && neighborStack.FreeDonutPlaces > 0)
                {
                    return MakeTransfer(from: cell, to: neighborCell, cell, neighborCell, donuts1, donuts2);
                }
                if (AreOtherTypesInStack(neighborCell, donuts2) && stack.FreeDonutPlaces > 0)
                {
                    return MakeTransfer(from: neighborCell, to: cell, cell, neighborCell, donuts1, donuts2);
                }
                if (neighborStack.FreeDonutPlaces > 0)
                {
                    // if (transfer.NeighBor.transform.localPosition.y > transfer.Cell.transform.localPosition.y)
                    // {
                    //     return MakeTransfer(from: cell, to: neighborCell, cell, neighborCell, donuts1, donuts2);
                    // }
                    return MakeTransfer(from: neighborCell, to: cell, cell, neighborCell, donuts1, donuts2);
                }
            }
            return null;
        }

        private Transfer MakeTransfer(Cell from, Cell to, Cell cell, Cell neighborCell, List<DonutType> donuts1, List<DonutType> donuts2)
        {
            Transfer transfer = new();
            transfer.From = from;
            transfer.To = to;
            transfer.Cell = cell;
            transfer.NeighBor = neighborCell;
            transfer.Priority = HighMidPriority(donuts1, donuts2);
            return transfer;
        }

        private Priority HighMidPriority(List<DonutType> donuts1, List<DonutType> donuts2)
        {
            if (StackCanBeFull(donuts1, donuts2))
            {
                return Priority.High;
            }

            return Priority.Medium;
        }

        private bool StackCanBeFull(List<DonutType> donuts1, List<DonutType> donuts2)
        {
            return donuts1.Count + donuts2.Count >= 3;
        }

        private bool AreOtherTypesInStack(Cell cell, List<DonutType> donuts)
        {
            return cell.DonutStack.DonutsCount > donuts.Count;
        }
    }
}