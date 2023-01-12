using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Donuts;
using UnityEngine;

namespace Level
{
    public class Transfer
    {
        private const float Offset = 0.3f;
        public Cell From { get; set; }
        public Cell To { get; set; }
        public Cell Cell { get; set; }
        public Cell NeighBor { get; set; }
        public Cell SimulatedCell { get; private set; }
        public Transfer AttachedTransfer { get; set; }
        public Priority Priority { get; set; }

        public void SimulateTransfer()
        {
            List<DonutType> fromDonuts = new(From.DonutStack.SimulatedDonuts);
            List<DonutType> toDonuts = new(To.DonutStack.SimulatedDonuts);
            while (toDonuts.Count < 3 && fromDonuts.Count > 1 && toDonuts[toDonuts.Count - 1] == fromDonuts[fromDonuts.Count - 1])
            {
                toDonuts.Add(fromDonuts[fromDonuts.Count - 1]);
                fromDonuts.Remove(toDonuts[toDonuts.Count - 1]);
            }
            if (fromDonuts.Count == 0)
            {
                TrySimulateDonutStacksMoving(From);
            }
            if (toDonuts.Count == 3)
            {
                List<DonutType> donuts = new List<DonutType>(toDonuts);
                List<DonutType> result = new List<DonutType>();
                donuts.Reverse();
                DonutType topDonutType = toDonuts[toDonuts.Count - 1];
                foreach (DonutType donut in donuts)
                {
                    if (donut == topDonutType)
                    {
                        result.Add(donut);
                    }
                    else
                    {
                        break;
                    }
                }
                if (result.Count == 3)
                    TrySimulateDonutStacksMoving(To);
            }
        }

        private void TrySimulateDonutStacksMoving(Cell cell)
        {
            Row row = cell.GetComponentInParent<Row>();
            int cellIndex = row.ActiveCells.IndexOf(cell) + 1;
            //if there's at least one active cell after release cell
            if (cellIndex < row.ActiveCells.Count)
            {
                List<Cell> activeCellsAfterReleaseCell = row.ActiveCells.Skip(cellIndex).ToList();
                SimulateDonutStackMove(activeCellsAfterReleaseCell, cell);
            }
        }

        private void SimulateDonutStackMove(List<Cell> activeCellsAfterReleaseCell, Cell cell)
        {
            Cell lastCell = cell;
            foreach (var c in activeCellsAfterReleaseCell)
            {
                lastCell.DonutStack.SimulatedDonuts = c.DonutStack.SimulatedDonuts;
                lastCell = c;
            }
            lastCell.DonutStack.SimulatedDonuts = null;
        }

        // public async Task TransferDonut()
        // {
        //     Donut fromTopDonut = From.DonutStack.GetTopDonut();
        //     Vector3 newPosition = new Vector3(To.DonutStack.GetTopDonut().transform.position.x,
        //                                      To.DonutStack.GetTopDonut().transform.position.y + Offset,
        //                                      To.DonutStack.GetTopDonut().transform.position.z);

        //     DoAnimation(From, To, fromTopDonut, newPosition);
        //     await Task.Delay(500);
        // }

        // private void DoAnimation(Cell from, Cell to, Donut fromTopDonut, Vector3 newPosition)
        // {
        //     if (IsTransferringLastDonut(from))
        //         GameObject.Destroy(from.DonutStack.Cylinder);
        //     fromTopDonut.transform.DOMove(newPosition, 0.5f).OnComplete(() =>
        //     CompleteTransfer(fromTopDonut, from.DonutStack, to.DonutStack));
        //     fromTopDonut.transform.DOScale(to.DonutStack.GetTopDonut().transform.localScale * 0.85f, 0.5f);
        // }

        // private static bool IsTransferringLastDonut(Cell from)
        // {
        //     return from.DonutStack.FreeDonutPlaces == 2;
        // }

        // private void CompleteTransfer(Donut fromTopDonut, DonutStack fromDonutStack, DonutStack toDonutStack)
        // {
        //     fromDonutStack.RemoveDonut(fromTopDonut);
        //     toDonutStack.AddDonut(fromTopDonut);
        //     fromTopDonut.transform.parent = toDonutStack.transform;
        // }
    }
}