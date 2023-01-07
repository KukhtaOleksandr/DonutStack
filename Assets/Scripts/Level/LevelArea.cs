using System.Collections.Generic;
using Zenject;

namespace Level
{
    public class LevelArea
    {
        [Inject] private List<Row> _rows;

        public List<Row> Rows { get => _rows; }

        public Dictionary<NeighborType, Cell> GetCellActiveNeighbors(Cell cell)
        {
            Row row = cell.GetComponentInParent<Row>();
            Dictionary<NeighborType, Cell> neighbors = new();
            int index = row.ActiveCells.IndexOf(cell);

            SetVerticalNeighbors(row, neighbors, index);

            SetSideNeighbors(row.LeftNeighbor, neighbors, NeighborType.Left, index);
            SetSideNeighbors(row.RightNeighbor, neighbors, NeighborType.Right, index);

            return neighbors;
        }

        private void SetVerticalNeighbors(Row row, Dictionary<NeighborType, Cell> neighbors, int index)
        {
            if (index != 0)
                neighbors.Add(NeighborType.Top, row.ActiveCells[index - 1]);
            if (index + 1 < row.ActiveCells.Count)
                neighbors.Add(NeighborType.Bottom, row.ActiveCells[index + 1]);
        }

        private void SetSideNeighbors(Row row, Dictionary<NeighborType, Cell> neighbors, NeighborType type, int index)
        {
            if (row != null)
            {
                if (index < row.ActiveCells.Count)
                    neighbors.Add(type, row.ActiveCells[index]);
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