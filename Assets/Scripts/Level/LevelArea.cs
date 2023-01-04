using System.Collections.Generic;

namespace Level
{
    public static class LevelArea
    {
        public static Dictionary<NeighborType, Cell> GetCellActiveNeighbors(Row row, Cell cell)
        {
            Dictionary<NeighborType, Cell> neighbors = new();
            int index = row.ActiveCells.IndexOf(cell);

            SetVerticalNeighbors(row, neighbors, index);

            SetSideNeighbors(row.LeftNeighbor, neighbors, NeighborType.Left, index);
            SetSideNeighbors(row.RightNeighbor, neighbors, NeighborType.Right, index);

            return neighbors;
        }

        private static void SetVerticalNeighbors(Row row, Dictionary<NeighborType, Cell> neighbors, int index)
        {
            if (index != 0)
                neighbors.Add(NeighborType.Top, row.ActiveCells[index - 1]);
            if (index + 1 < row.ActiveCells.Count)
                neighbors.Add(NeighborType.Bottom, row.ActiveCells[index + 1]);
        }

        private static void SetSideNeighbors(Row row, Dictionary<NeighborType, Cell> neighbors, NeighborType type, int index)
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