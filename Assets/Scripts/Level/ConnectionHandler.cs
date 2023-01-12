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

            Path path = _pathMaker.GetPath(CurrentCell, null);
            if (path != null)
            {
                bool isFirstTransfer = false;
                while (path.Connection.Count > 0)
                {
                    Transfer transfer = path.Connection.Dequeue();
                    if (isFirstTransfer == false)
                    {
                        path.InvolvedCells[transfer.Cell] = transfer.NeighBor;
                        isFirstTransfer = true;
                    }
                    while (transfer.From.CanTransferDonutTo(transfer.To))
                        await TransferDonut(transfer.From, transfer.To);
                }
                _levelArea.ResetSimulatedDonuts();

                while (path.InvolvedCells.Count > 0)
                {
                    var cell = path.InvolvedCells.First();
                    if (cell.Key.DonutStack != null)
                    {
                        if (cell.Key.DonutStack.FreeDonutPlaces < 3 && cell.Key.DonutStack.IsFull() == false)
                        {
                            Dictionary<NeighborType, Cell> neighbors1 = _levelArea.GetCellActiveNeighbors(cell.Key, cell.Value).
                            Where(n => n.Value.DonutStack.GetTopDonut().Type == cell.Key.DonutStack.GetTopDonut().Type).
                            ToDictionary(x => x.Key, x => x.Value);
                            if (neighbors1.Count == 0)
                            {
                                path.InvolvedCells.Remove(cell.Key);
                                continue;
                            }

                            bool isNeighborWithFreeDonutPlaces = false;
                            foreach (var n in neighbors1)
                            {
                                if (n.Value.DonutStack.FreeDonutPlaces > 0)
                                {
                                    isNeighborWithFreeDonutPlaces = true;
                                    break;
                                }
                            }
                            if (isNeighborWithFreeDonutPlaces || cell.Key.DonutStack.FreeDonutPlaces > 0)
                            {
                                Path path1 = _pathMaker.GetPath(cell.Key, cell.Value);

                                //path.InvolvedCells = involvedCells.Merge();

                                if (path1 != null)
                                {
                                    path.InvolvedCells = path.InvolvedCells.Concat(path1.InvolvedCells.Where(x => !path.InvolvedCells.Keys.
                                    Contains(x.Key))).ToDictionary(x => x.Key, x => x.Value);
                                    while (path1.Connection.Count > 0)
                                    {
                                        Transfer transfer = path1.Connection.Dequeue();
                                        while (transfer.From.CanTransferDonutTo(transfer.To))
                                            await TransferDonut(transfer.From, transfer.To);
                                    }
                                }
                                _levelArea.ResetSimulatedDonuts();
                            }
                        }
                    }
                    path.InvolvedCells.Remove(cell.Key);
                    if (path.InvolvedCells.Count == 0)
                        break;
                    if (cell.Key.DonutStack != null)
                    {
                        if (cell.Key.DonutStack.FreeDonutPlaces < 3 && cell.Key.DonutStack.IsFull() == false)
                        {
                            try
                            {
                                path.InvolvedCells.Add(cell.Key, cell.Value);
                            }
                            catch
                            {
                                break;
                            }
                        }
                    }
                    //last = cell;
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
            await CompleteTransfer(from, fromTopDonut, from.DonutStack, to.DonutStack);
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

        private async Task CompleteTransfer(Cell from, Donut fromTopDonut, DonutStack fromDonutStack, DonutStack toDonutStack)
        {
            if (IsTransferringLastDonut(from))
            {
                await fromDonutStack.RemoveDonut(fromTopDonut, toDonutStack);
                await toDonutStack.AddDonut(fromTopDonut);
            }
            else
            {
                await fromDonutStack.RemoveDonut(fromTopDonut, toDonutStack);
                //fromTopDonut.transform.parent = toDonutStack.transform;
                await toDonutStack.AddDonut(fromTopDonut);
            }
        }

        private bool DonutsTypesEqual(Cell cell)
        {
            return cell.DonutStack.GetTopDonut().Type == CurrentCell.DonutStack.GetTopDonut().Type;
        }
    }
}