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
        private const float Duration = 0.15f;

        public Cell From { get; set; }
        public Cell To { get; set; }
        public Cell Cell { get; set; }
        public Cell NeighBor { get; set; }
        public NeighborType NeighborType { get; set; }
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

        public async Task TransferDonut()
        {
            Donut fromTopDonut = From.DonutStack.GetTopDonut();
            Vector3 newPosition = new Vector3(To.DonutStack.GetTopDonut().transform.position.x,
                                             To.DonutStack.GetTopDonut().transform.position.y + Offset,
                                             To.DonutStack.GetTopDonut().transform.position.z);

            await DoAnimation(From, To, fromTopDonut, newPosition);

        }

        private async Task DoAnimation(Cell from, Cell to, Donut fromTopDonut, Vector3 newPosition)
        {
            if (IsTransferringLastDonut(from))
                GameObject.Destroy(from.DonutStack.Cylinder);
            Vector3 firstPosition = GetFirstPosition(fromTopDonut.transform.position, newPosition);
            await DoTransferAnimation(to, fromTopDonut, newPosition, firstPosition);

            await CompleteTransfer(From, fromTopDonut, From.DonutStack, To.DonutStack);
        }


        private Vector3 GetFirstPosition(Vector3 start, Vector3 end)
        {
            Vector3 result = start - end;
            if (NeighborType == NeighborType.Top || NeighborType == NeighborType.Bottom)
            {
                result = new Vector3(start.x, end.y * 2.5f, result.z / 2 + end.z);
            }
            else
            {
                result = new Vector3(result.x / 2 + end.x, end.y * 2.5f, start.z);
            }
            return result;
        }

        private Vector3 GetRotation()
        {
            if (NeighborType == NeighborType.Top)
            {
                return new Vector3(-90, 0, 0);
            }
            if (NeighborType == NeighborType.Bottom)
            {
                return new Vector3(90, 0, 0);
            }
            if (NeighborType == NeighborType.Right)
            {
                return new Vector3(0, 0, -90);
            }
            else
            {
                return new Vector3(0, 0, 90);
            }
        }

        private async Task DoTransferAnimation(Cell to, Donut fromTopDonut, Vector3 newPosition, Vector3 firstPosition)
        {
            TweenParams moveScaleParams = new TweenParams().SetEase(Ease.InSine);
            Sequence moveScale = DOTween.Sequence();
            Vector3 rotation = GetRotation();
            moveScale.Append(fromTopDonut.transform.DOMove(firstPosition, Duration));
            moveScale.Append(fromTopDonut.transform.DOMove(newPosition, Duration));
            moveScale.Insert(0, fromTopDonut.transform.DOScale(to.DonutStack.GetTopDonut().transform.localScale * 0.85f, moveScale.Duration()));
            moveScale.SetAs(moveScaleParams);

            TweenParams rotateParams = new TweenParams().SetEase(Ease.InSine).SetRelative(true);
            Sequence rotate = DOTween.Sequence();
            rotate.Append(fromTopDonut.transform.DORotate(rotation, Duration, RotateMode.WorldAxisAdd));
            rotate.Append(fromTopDonut.transform.DORotate(rotation, Duration, RotateMode.WorldAxisAdd));
            rotate.SetAs(rotateParams);
            await Task.Delay(300);
        }

        private bool IsTransferringLastDonut(Cell from)
        {
            return from.DonutStack.FreeDonutPlaces == 2;
        }

        private async Task CompleteTransfer(Cell from, Donut fromTopDonut, DonutStack fromDonutStack, DonutStack toDonutStack)
        {
            await DoSquishAnimation(fromTopDonut, toDonutStack);
            await TryDoFullAnimation(toDonutStack, fromTopDonut);
            await fromDonutStack.RemoveDonut(fromTopDonut, toDonutStack);
            await toDonutStack.AddDonut(fromTopDonut);
        }

        private static async Task TryDoFullAnimation(DonutStack donutStack, Donut fromTopDonut)
        {
            List<Donut> donuts = donutStack.GetTopDonutsOfOneType();
            DonutType donutType = donuts.First().Type;
            if (donuts.Count == 2)
            {
                if (donutType == fromTopDonut.Type)
                {
                    donutStack.transform.DOScale(Vector3.zero, 0.15f);
                    await Task.Delay(150);
                }
            }
        }

        private async Task DoSquishAnimation(Donut fromTopDonut, DonutStack toDonutStack)
        {
            fromTopDonut.transform.parent = toDonutStack.transform;
            TweenParams scaleParams = new TweenParams().SetEase(Ease.Linear);
            Sequence scale = DOTween.Sequence();
            Vector3 firstScale = new Vector3(1.2f, 0.8f, 1.2f);
            scale.Append(toDonutStack.transform.DOScale(firstScale, 0.075f));
            scale.Append(toDonutStack.transform.DOScale(new Vector3(1, 1, 1), 0.075f));
            scale.SetAs(scaleParams);
            await Task.Delay(150);
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

    }
}