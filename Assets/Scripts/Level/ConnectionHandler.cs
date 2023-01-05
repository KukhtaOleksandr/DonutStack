using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Donuts;
using Donuts.StateMachine;
using UnityEngine;
using Zenject;

namespace Level
{
    public class ConnectionHandler : IInitializable, IDisposable
    {
        private const float Offset = 0.3f;

        [Inject] SignalBus _signalBus;
        [Inject] LevelArea _levelArea;
        public Row CurrentRow { get; set; }
        public Cell CurrentCell { get; set; }

        public void Initialize()
        {
            _signalBus.Subscribe<SignalDonutMovedToPosition>(GetPath);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<SignalDonutMovedToPosition>(GetPath);
        }

        private async void GetPath()
        {
            Dictionary<NeighborType, Cell> neighbors = _levelArea.GetCellActiveNeighbors(CurrentRow, CurrentCell);

            if (neighbors.TryGetValue(NeighborType.Top, out Cell cell))
            {
                if (DonutsTypesEqual(cell))
                {
                    while (cell.DonutStack.FreeDonutPlaces > 0 && CurrentCell.DonutStack.FreeDonutPlaces<3)
                        await TransferDonut(from: CurrentCell, to: cell);
                }
            }
            _signalBus.Fire<SignalConnectionFinished>();
        }

        private async Task TransferDonut(Cell from, Cell to)
        {
            Donut fromTopDonut = from.DonutStack.GetTopDonut();
            Vector3 newPosition = new Vector3(to.DonutStack.GetTopDonut().transform.position.x,
                                             to.DonutStack.GetTopDonut().transform.position.y + Offset,
                                             to.DonutStack.GetTopDonut().transform.position.z);

            DoAnimation(from, to, fromTopDonut, newPosition);
            await Task.Delay(500);
        }

        private void DoAnimation(Cell from, Cell to, Donut fromTopDonut, Vector3 newPosition)
        {
            if (IsTransferringLastDonut(from))
                GameObject.Destroy(from.DonutStack.Cylinder);
            fromTopDonut.transform.DOMove(newPosition, 0.5f).OnComplete(() => CompleteTransfer(fromTopDonut, from.DonutStack, to.DonutStack));
            fromTopDonut.transform.DOScale(to.DonutStack.GetTopDonut().transform.localScale * 0.85f, 0.5f);
        }

        private static bool IsTransferringLastDonut(Cell from)
        {
            return from.DonutStack.FreeDonutPlaces == 2;
        }

        private void CompleteTransfer(Donut fromTopDonut, DonutStack fromDonutStack, DonutStack toDonutStack)
        {
            fromDonutStack.RemoveDonut(fromTopDonut);
            toDonutStack.AddDonut(fromTopDonut);
            fromTopDonut.transform.parent = toDonutStack.transform;
        }

        private bool DonutsTypesEqual(Cell cell)
        {
            return cell.DonutStack.GetTopDonut().Type == CurrentCell.DonutStack.GetTopDonut().Type;
        }
    }
}