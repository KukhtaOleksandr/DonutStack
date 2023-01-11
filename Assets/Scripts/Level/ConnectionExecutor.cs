using System;
using Donuts;
using UnityEngine;
using Zenject;

namespace Level
{
    public class ConnectionExecutor : IInitializable, IDisposable
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private LevelArea _levelArea;

        public void Initialize()
        {
            //_signalBus.Subscribe<SignalDonutStackIsEmpty>(ExecuteConnection);
            //_signalBus.Subscribe<SignalDonutStackIsFull>(ExecuteConnection);
        }

        public void Dispose()
        {
            //_signalBus.Unsubscribe<SignalDonutStackIsEmpty>(ExecuteConnection);
            //_signalBus.Unsubscribe<SignalDonutStackIsFull>(ExecuteConnection);
        }

        public void ExecuteConnection(SignalDonutStackIsEmpty args)
        {
            Row row = _levelArea.Rows.Find(row => row.ActiveCells.Find(c => c.DonutStack == args.DonutStack));
            Cell cell = row.ActiveCells.Find(c => c.DonutStack == args.DonutStack);
            // Cell cell = args.DonutStack.GetComponentInParent<Cell>();
            // Row row = cell.GetComponentInParent<Row>();
            
            row.ReleaseCellAndMoveOthers(cell);
        }

        public void ExecuteConnection(SignalDonutStackIsFull args)
        {
            Row row = _levelArea.Rows.Find(row => row.ActiveCells.Find(c => c.DonutStack == args.DonutStack));
            Cell cell = row.ActiveCells.Find(c => c.DonutStack == args.DonutStack);

            // Cell cell = args.DonutStack.GetComponentInParent<Cell>();
            // Row row = cell.GetComponentInParent<Row>();
            row.ReleaseCellAndMoveOthers(cell);
        }

    }
}