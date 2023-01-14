using System.Collections.Generic;
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


        private void SetVerticalNeighbors(Row row, Dictionary<NeighborType, Cell> neighbors, Cell from, int index)
        {
            if (index != 0)
            {
                Cell cell = row.ActiveCells[index - 1];
                if (cell != from || cell.isMergingGrantsFullStack(from))
                    neighbors.Add(NeighborType.Top, cell);
            }
            if (index + 1 < row.ActiveCells.Count)
            {
                Cell cell = row.ActiveCells[index + 1];
                if (cell != from || cell.isMergingGrantsFullStack(from))
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
                    if (cell != from || cell.isMergingGrantsFullStack(from))
                        neighbors.Add(type, row.ActiveCells[index]);
                }
            }
        }
    }

}