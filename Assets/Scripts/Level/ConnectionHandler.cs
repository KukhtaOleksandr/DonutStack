using System;
using System.Collections.Generic;
using System.Linq;
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
        [Inject] PathMaker _pathMaker;
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
            Dictionary<NeighborType, Cell> neighbors = _levelArea.GetCellActiveNeighbors(CurrentCell, null).
            Where(n => n.Value.DonutStack.GetTopDonut().Type == CurrentCell.DonutStack.GetTopDonut().Type).
            ToDictionary(x => x.Key, x => x.Value);

            if (neighbors.Count == 0)
            {
                _signalBus.Fire<SignalConnectionFinished>();
                return;
            }

            Queue<Transfer> path = _pathMaker.GetPath(CurrentCell);
            if (path != null)
            {
                while (path.Count > 0)
                {
                    Transfer transfer = path.Dequeue();
                    while (transfer.From.CanTransferDonutTo(transfer.To))
                        await TransferDonut(transfer.From, transfer.To);
                }
                _levelArea.ResetSimulatedDonuts();
                if (CurrentCell.DonutStack.FreeDonutPlaces < 3)
                {
                    Dictionary<NeighborType, Cell> neighbors1 = _levelArea.GetCellActiveNeighbors(CurrentCell, null).
                    Where(n => n.Value.DonutStack.GetTopDonut().Type == CurrentCell.DonutStack.GetTopDonut().Type).
                    ToDictionary(x => x.Key, x => x.Value);

                    bool isNeighborWithFreeDonutPlaces = false;
                    foreach (var n in neighbors1)
                    {
                        if (n.Value.DonutStack.FreeDonutPlaces > 0)
                        {
                            isNeighborWithFreeDonutPlaces = true;
                            break;
                        }
                    }
                    if (isNeighborWithFreeDonutPlaces)
                    {
                        Queue<Transfer> path1 = _pathMaker.GetPath(CurrentCell);
                        if (path1 != null)
                            while (path1.Count > 0)
                            {
                                Transfer transfer = path.Dequeue();
                                while (transfer.From.CanTransferDonutTo(transfer.To))
                                    await TransferDonut(transfer.From, transfer.To);
                            }
                        _levelArea.ResetSimulatedDonuts();
                    }
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
            CompleteTransfer(from, fromTopDonut, from.DonutStack, to.DonutStack);
        }

        private void DoAnimation(Cell from, Cell to, Donut fromTopDonut, Vector3 newPosition)
        {
            if (IsTransferringLastDonut(from))
                GameObject.Destroy(from.DonutStack.Cylinder);
            fromTopDonut.transform.DOMove(newPosition, 0.5f);
            fromTopDonut.transform.DOScale(to.DonutStack.GetTopDonut().transform.localScale * 0.85f, 0.5f);
        }

        private bool IsTransferringLastDonut(Cell from)
        {
            return from.DonutStack.FreeDonutPlaces == 2;
        }

        private async void CompleteTransfer(Cell from, Donut fromTopDonut, DonutStack fromDonutStack, DonutStack toDonutStack)
        {
            if (IsTransferringLastDonut(from))
            {
                await toDonutStack.AddDonut(fromTopDonut);
                await fromDonutStack.RemoveDonut(fromTopDonut);
            }
            else
            {
                await fromDonutStack.RemoveDonut(fromTopDonut);
                await toDonutStack.AddDonut(fromTopDonut);
            }
            fromTopDonut.transform.parent = toDonutStack.transform;
        }

        private bool DonutsTypesEqual(Cell cell)
        {
            return cell.DonutStack.GetTopDonut().Type == CurrentCell.DonutStack.GetTopDonut().Type;
        }
    }
}