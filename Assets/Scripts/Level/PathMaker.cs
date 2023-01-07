using System.Collections.Generic;
using System.Linq;
using Donuts;
using Zenject;

namespace Level
{
    public static class PathMaker
    {
        [Inject] private static LevelArea _levelArea;
        //private List<Cell> _path;

        public static void GetPath(Cell cell)
        {
            Dictionary<NeighborType, Cell> activeNeighbors = _levelArea.GetCellActiveNeighbors(cell);
            activeNeighbors = (Dictionary<NeighborType, Cell>)activeNeighbors.Where(
                n => n.Value.DonutStack.GetTopDonut().Type == cell.DonutStack.GetTopDonut().Type);

            Dictionary<Cell, int> cellsValue = new();
            List<Cell> firstPriority = new();
            foreach (var c in activeNeighbors)
            {
                Transfer transfer = new();
                
            }

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
                        transfer.From = cell; transfer.To = neighborCell; transfer.Bipolar = false;
                    }
                    if (donuts1.Count < donuts2.Count)
                    {
                        transfer.From = neighborCell; transfer.To = cell; transfer.Bipolar = false;
                    }
                    else
                    {
                        transfer.From = neighborCell; transfer.To = cell; transfer.Bipolar = true;
                    }
                    return Priority.Low;
                }
                if (AreOtherTypesInStack(cell, donuts1) && neighborStack.FreeDonutPlaces > 0)
                {
                    transfer.From = cell;
                    transfer.To = neighborCell;
                    transfer.Bipolar = false;
                    return HighMidPriority(donuts1, donuts2);
                }
                if (AreOtherTypesInStack(neighborCell, donuts2) && stack.FreeDonutPlaces > 0)
                {
                    transfer.From = neighborCell;
                    transfer.To = cell;
                    transfer.Bipolar = false;
                    return HighMidPriority(donuts1, donuts2);
                }
                if (neighborStack.FreeDonutPlaces > 0)
                {
                    transfer.From = cell;
                    transfer.To = neighborCell;
                    transfer.Bipolar = true;
                    return HighMidPriority(donuts1, donuts2);
                }
                transfer = null;
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