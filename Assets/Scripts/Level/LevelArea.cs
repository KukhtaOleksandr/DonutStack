using System.Collections.Generic;
using System.Threading.Tasks;
using Donuts;
using Zenject;

namespace Level
{
    public class LevelArea
    {
        [Inject] private List<Row> _rows;

        public List<Row> Rows { get => _rows; }


        public Dictionary<NeighborType, Cell> GetCellActiveNeighbors(Cell cell, Cell from)
        {
            Row row = cell.GetComponentInParent<Row>();
            Dictionary<NeighborType, Cell> neighbors = new();
            int index = row.ActiveCells.IndexOf(cell);

            SetVerticalNeighbors(row, neighbors, from, index);

            SetSideNeighbors(row.LeftNeighbor, neighbors, from, NeighborType.Left, index);
            SetSideNeighbors(row.RightNeighbor, neighbors, from, NeighborType.Right, index);

            return neighbors;
        }

        public void ResetSimulatedDonuts()
        {
            foreach (Row row in _rows)
            {
                foreach (Cell cell in row.ActiveCells)
                {
                    cell.DonutStack.ResetSimulatedDonuts();
                }
            }
        }

        public async Task HandleDonutsChange(DonutStack donutStack)
        {
            if (donutStack.FreeDonutPlaces == 3 || (donutStack.FreeDonutPlaces == 0 && donutStack.GetTopDonutsOfOneType().Count == 3))
            {
                Row row = Rows.Find(row => row.ActiveCells.Find(c => c.DonutStack == donutStack));
                Cell cell = row.ActiveCells.Find(c => c.DonutStack == donutStack);
                await row.ReleaseCellAndMoveOthers(cell);
            }
        }

        private void SetVerticalNeighbors(Row row, Dictionary<NeighborType, Cell> neighbors, Cell from, int index)
        {
            if (index != 0)
            {
                Cell cell = row.ActiveCells[index - 1];
                if (cell != from)
                    neighbors.Add(NeighborType.Top, cell);
            }
            if (index + 1 < row.ActiveCells.Count)
            {
                Cell cell = row.ActiveCells[index + 1];
                if (cell != from)
                    neighbors.Add(NeighborType.Bottom, cell);
            }
        }

        private void SetSideNeighbors(Row row, Dictionary<NeighborType, Cell> neighbors, Cell from, NeighborType type, int index)
        {
            if (row != null)
            {
                if (index < row.ActiveCells.Count)
                {
                    Cell cell = row.ActiveCells[index];
                    if (cell != from)
                        neighbors.Add(type, row.ActiveCells[index]);
                }
            }
        }
    }

    public enum NeighborType
    {
        Left,
        Right,
        Top,
        Bottom
    }

}