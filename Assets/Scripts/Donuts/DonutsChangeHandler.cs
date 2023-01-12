using System;
using System.Threading.Tasks;
using Level;
using Zenject;

namespace Donuts
{
    public class DonutsChangeHandler 
    {
        [Inject] private LevelArea _levelArea;

        public async Task Handle(DonutStack donutStack)
        {
            if (donutStack.FreeDonutPlaces == 3 || (donutStack.FreeDonutPlaces == 0 && donutStack.GetTopDonutsOfOneType().Count == 3))
            {
                Row row = _levelArea.Rows.Find(row => row.ActiveCells.Find(c => c.DonutStack == donutStack));
                Cell cell = row.ActiveCells.Find(c => c.DonutStack == donutStack);
                await row.ReleaseCellAndMoveOthers(cell);
            }
        }

    }
}