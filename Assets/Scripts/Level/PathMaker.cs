using System.Collections.Generic;
using System.Linq;
using Donuts;
using Zenject;

namespace Level
{
    public static class PathMaker
    {
        [Inject] private static LevelArea _levelArea;

        private static Queue<Transfer> connection;

        public static Queue<Transfer> GetPath(Cell cell)
        {
            connection = new();
            Transfer transfer = GetConnection(cell);
            while(transfer.Cell!=cell)
            {
                connection.Enqueue(transfer);
                transfer=transfer.AttachedTransfer;
            }
            return connection;
        }

        private static Transfer GetConnection(Cell cell)
        {
            Dictionary<NeighborType, Cell> activeNeighbors = _levelArea.GetCellActiveNeighbors(cell);
            activeNeighbors = (Dictionary<NeighborType, Cell>)activeNeighbors.Where(
                n => n.Value.DonutStack.GetTopDonut().Type == cell.DonutStack.GetTopDonut().Type);
            if (activeNeighbors.Count == 0)
                return null;

            Dictionary<Transfer, Priority> priorities = new();

            foreach (var neighbor in activeNeighbors)
            {
                Transfer transfer;
                Priority priority = GetPriority(cell, neighbor.Value, out transfer);
                priorities.Add(transfer, priority);
            }

            Dictionary<Transfer, Priority> highestPriorities = GetHighestPriorities(priorities);
            if (highestPriorities.Count > 1)
            {
                return GetSubConnection(highestPriorities);
            }
            else
            {
                return highestPriorities.First().Key;
            }
        }

        private static Transfer GetSubConnection(Dictionary<Transfer, Priority> highestPriorities)
        {
            List<Dictionary<Transfer, Priority>> highestSubPriorities = new();
            foreach (var h in highestPriorities)
            {
                Dictionary<NeighborType, Cell> activeNeighbors = _levelArea.GetCellActiveNeighbors(h.Key.Cell);
                activeNeighbors = (Dictionary<NeighborType, Cell>)activeNeighbors.Where(
                    n => n.Value.DonutStack.GetTopDonut().Type == h.Key.Cell.DonutStack.GetTopDonut().Type);
                if (activeNeighbors.Count == 0)
                    return null;

                Dictionary<Transfer, Priority> priorities = new();

                foreach (var neighbor in activeNeighbors)
                {
                    Transfer transfer;
                    Priority priority = GetPriority(h.Key.Cell, neighbor.Value, out transfer);
                    priorities.Add(transfer, priority);
                }

                Dictionary<Transfer, Priority> localPriorities = GetHighestPriorities(priorities);
                foreach (var p in localPriorities)
                {
                    p.Key.AttachedTransfer = h.Key;
                    highestSubPriorities.Add(localPriorities);
                }
            }

            highestSubPriorities = GetHighestPriorities(highestSubPriorities);
            if (highestSubPriorities.Count > 1)
            {
                Dictionary<Transfer, Priority> newPriorities = new();
                foreach (var d in highestSubPriorities)
                {
                    foreach (var v in d)
                    {
                        newPriorities.Add(v.Key, v.Value);
                    }
                }
                return GetSubConnection(newPriorities);
            }
            else if (highestSubPriorities.Count == 0)
            {
                return highestPriorities.First().Key;
            }
            else
            {
                return highestSubPriorities[0].First().Key;
            }

        }

        private static List<Dictionary<Transfer, Priority>> GetHighestPriorities(List<Dictionary<Transfer, Priority>> priorities)
        {
            List<Dictionary<Transfer, Priority>> result = (List<Dictionary<Transfer, Priority>>)
            priorities.Where(p => p.First().Value == Priority.High);

            if (result.Count == 0)
                result = (List<Dictionary<Transfer, Priority>>)
                priorities.Where(p => p.First().Value == Priority.Medium);
            if (result.Count == 0)
                result = (List<Dictionary<Transfer, Priority>>)
                priorities.Where(p => p.First().Value == Priority.Low);
            return result;
        }

        private static Dictionary<Transfer, Priority> GetHighestPriorities(Dictionary<Transfer, Priority> priorities)
        {
            Dictionary<Transfer, Priority> result = (Dictionary<Transfer, Priority>)priorities.Where(p => p.Value == Priority.High);
            if (result.Count == 0)
                result = (Dictionary<Transfer, Priority>)priorities.Where(p => p.Value == Priority.Medium);
            if (result.Count == 0)
                result = (Dictionary<Transfer, Priority>)priorities.Where(p => p.Value == Priority.Low);
            return result;
        }

        private static Priority GetPriority(Cell cell, Cell neighborCell, out Transfer transfer)
        {
            transfer = new Transfer();
            DonutStack stack = cell.DonutStack;
            DonutStack neighborStack = neighborCell.DonutStack;

            List<Donut> donuts1 = cell.DonutStack.GetTopDonutsOfOneType();
            List<Donut> donuts2 = neighborCell.DonutStack.GetTopDonutsOfOneType();

            if ((stack.FreeDonutPlaces > 0 || neighborStack.FreeDonutPlaces > 0))
            {
                if (AreOtherTypesInStack(cell, donuts1) && AreOtherTypesInStack(neighborCell, donuts2))
                {
                    if (donuts1.Count > donuts2.Count)
                    {
                        transfer.From = cell; transfer.To = neighborCell;
                    }
                    if (donuts1.Count < donuts2.Count)
                    {
                        transfer.From = neighborCell; transfer.To = cell;
                    }
                    else
                    {
                        transfer.From = neighborCell; transfer.To = cell;
                    }
                    transfer.Cell = cell;
                    transfer.NeighBor = neighborCell;
                    return Priority.Low;
                }
                if (AreOtherTypesInStack(cell, donuts1) && neighborStack.FreeDonutPlaces > 0)
                {
                    transfer.From = cell;
                    transfer.To = neighborCell;
                    transfer.Cell = cell;
                    transfer.NeighBor = neighborCell;
                    return HighMidPriority(donuts1, donuts2);
                }
                if (AreOtherTypesInStack(neighborCell, donuts2) && stack.FreeDonutPlaces > 0)
                {
                    transfer.From = neighborCell;
                    transfer.To = cell;
                    transfer.Cell = cell;
                    transfer.NeighBor = neighborCell;
                    return HighMidPriority(donuts1, donuts2);
                }
                if (neighborStack.FreeDonutPlaces > 0)
                {
                    transfer.From = neighborCell;
                    transfer.To = cell;
                    transfer.Cell = cell;
                    transfer.NeighBor = neighborCell;
                    return HighMidPriority(donuts1, donuts2);
                }
            }
            transfer = null;
            return Priority.None;
        }

        private static Priority HighMidPriority(List<Donut> donuts1, List<Donut> donuts2)
        {
            if (StackCanBeFull(donuts1, donuts2))
                return Priority.High;

            return Priority.Medium;
        }

        private static bool StackCanBeFull(List<Donut> donuts1, List<Donut> donuts2)
        {
            return donuts1.Count + donuts2.Count >= 3;
        }

        private static bool AreOtherTypesInStack(Cell cell, List<Donut> donuts)
        {
            return cell.DonutStack.DonutsCount > donuts.Count;
        }
    }
}