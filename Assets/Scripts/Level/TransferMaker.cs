using System.Collections.Generic;
using Donuts;

namespace Level
{
    public class TransferMaker
    {
        public Transfer Make(Cell cell, Cell neighborCell)
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