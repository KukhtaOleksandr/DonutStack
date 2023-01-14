using System.Collections.Generic;
using Donuts;
using UnityEngine;

namespace Level
{
    public class TransferMaker
    {
        public Transfer Make(Cell cell, Cell neighborCell, NeighborType neighborType)
        {
            Transfer transfer = new Transfer();
            DonutStack stack = cell.DonutStack;
            DonutStack neighborStack = neighborCell.DonutStack;
            transfer.NeighborType = neighborType;
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
                        GetNeighborType(neighborType, cell.transform.position, neighborCell.transform.position);
                    }
                    else if (donuts1.Count < donuts2.Count)
                    {
                        transfer.From = neighborCell; transfer.To = cell;
                        transfer.NeighborType = GetNeighborType(neighborType, neighborCell.transform.position, cell.transform.position);
                    }
                    else
                    {
                        if (transfer.Cell.DonutStack.FreeDonutPlaces > transfer.NeighBor.DonutStack.FreeDonutPlaces)
                        {
                            transfer.From = neighborCell; transfer.To = cell;
                            transfer.NeighborType = GetNeighborType(neighborType, neighborCell.transform.position, cell.transform.position);
                        }
                        else if (transfer.Cell.DonutStack.FreeDonutPlaces < transfer.NeighBor.DonutStack.FreeDonutPlaces)
                        {
                            GetNeighborType(neighborType, cell.transform.position, neighborCell.transform.position);
                            transfer.From = cell; transfer.To = neighborCell;
                        }
                        else
                        {
                            //TODO: return best transfer
                            transfer.NeighborType = GetNeighborType(neighborType, neighborCell.transform.position, cell.transform.position);
                            transfer.From = neighborCell; transfer.To = cell;
                        }
                    }
                    return transfer;
                }
                if (AreOtherTypesInStack(cell, donuts1) && neighborStack.FreeDonutPlaces > 0)
                {
                    transfer.NeighborType = GetNeighborType(neighborType, cell.transform.position, neighborCell.transform.position);
                    return MakeTransfer(transfer, from: cell, to: neighborCell, cell, neighborCell, donuts1, donuts2);
                }
                if (AreOtherTypesInStack(neighborCell, donuts2) && stack.FreeDonutPlaces > 0)
                {
                    transfer.NeighborType = GetNeighborType(neighborType, neighborCell.transform.position, cell.transform.position);
                    return MakeTransfer(transfer, from: neighborCell, to: cell, cell, neighborCell, donuts1, donuts2);
                }
                if (neighborStack.FreeDonutPlaces > 0)
                {
                    transfer.NeighborType = GetNeighborType(neighborType, neighborCell.transform.position, cell.transform.position);
                    return MakeTransfer(transfer, from: neighborCell, to: cell, cell, neighborCell, donuts1, donuts2);
                }
            }
            return null;
        }

        private NeighborType GetNeighborType(NeighborType neighborType, Vector3 from, Vector3 to)
        {
            if (neighborType == NeighborType.Left || neighborType == NeighborType.Right)
            {
                if (from.x > to.x)
                    return NeighborType.Left;
                return NeighborType.Right;
            }
            else
            {
                if (from.z > to.z)
                    return NeighborType.Top;
                return NeighborType.Bottom;
            }
        }

        private Transfer MakeTransfer(Transfer transfer, Cell from, Cell to, Cell cell, Cell neighborCell, List<DonutType> donuts1, List<DonutType> donuts2)
        {
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