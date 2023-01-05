using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Level
{
    public class Row : MonoBehaviour
    {
        [SerializeField] private List<Cell> _cells;
        [SerializeField] private Row _leftNeighbor;
        [SerializeField] private Row _rightNeighbor;

        public Row LeftNeighbor { get => _leftNeighbor; }
        public Row RightNeighbor { get => _rightNeighbor; }
        public List<Cell> ActiveCells { get; private set; }

        private List<Cell> _freeCells;


        void Start()
        {
            ActiveCells = new();
            SetCellsPositions();
        }

        public Cell GetFreeCell()
        {
            Cell result = _freeCells[0];
            ActiveCells.Add(result);
            _freeCells.Remove(result);

            return result;
        }

        public void ReleaseCellAndMoveOthers(Cell cell)
        {
            cell.ReleaseCell();
            int index = ActiveCells.IndexOf(cell) + 1;
            //if there's at least one active cell after release cell
            if (index < ActiveCells.Count)
            {
                List<Cell> activeCellsAfterReleaseCell = ActiveCells.Skip(index).ToList();
                Cell lastCell = cell;

                foreach (Cell cellAfter in activeCellsAfterReleaseCell)
                {
                    cellAfter.MoveDonutToAnotherCell(lastCell);
                    lastCell = cellAfter;
                }

                ActiveCells.Remove(lastCell);
                _freeCells.Insert(0,lastCell);
            }
            else
            {
                ActiveCells.Remove(cell);
                _freeCells.Insert(0,cell);
            }
        }

        private void SetCellsPositions()
        {
            _freeCells = _cells.OrderByDescending(x => x.transform.localPosition.y).ToList();
        }
    }
}