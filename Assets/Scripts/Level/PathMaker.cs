using System.Collections.Generic;
using System.Linq;
using Donuts;
using Zenject;

namespace Level
{
    public class PathMaker
    {
        [Inject] private LevelArea _levelArea;

        private Queue<Transfer> connection;

        public Queue<Transfer> GetPath(Cell cell)
        {
            connection = new();
            Transfer transfer = GetConnection(cell);
            if (transfer == null)
            {
                return null;
            }
            if (transfer.AttachedTransfer == null)
            {
                connection.Enqueue(transfer);
                return connection;
            }

            while (transfer.Cell != cell)
            {
                connection.Enqueue(transfer);
                transfer = transfer.AttachedTransfer;
            }
            return connection;
        }

        private Transfer GetConnection(Cell cell)
        {
            Dictionary<NeighborType, Cell> activeNeighbors = _levelArea.GetCellActiveNeighbors(cell, null);
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
                return highestPriorities.First();
            }
        }

        private Transfer GetSubConnection(List<Transfer> highestPriorities, Cell from)
        {
            List<List<Transfer>> highestSubPriorities = new();
            foreach (var h in highestPriorities)
            {
                Dictionary<NeighborType, Cell> activeNeighbors = _levelArea.GetCellActiveNeighbors(h.NeighBor, from);
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
                    return null;

                List<Transfer> localPriorities = GetHighestPriorities(priorities);
                foreach (var p in localPriorities)
                {
                    p.AttachedTransfer = h;
                    highestSubPriorities.Add(localPriorities);
                }
            }

            highestSubPriorities = GetHighestPriorities(highestSubPriorities);
            if (highestSubPriorities.Count > 1)
            {
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