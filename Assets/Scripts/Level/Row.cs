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

        private List<Cell> _cellsSorted;


        void Start()
        {
            ActiveCells=new();
            SetCellsPositions();
        }

        public Cell GetFreeCell()
        {
            Cell result = _cellsSorted[0];
            result.IsActive = true;
            ActiveCells.Add(result);
            _cellsSorted.Remove(result);

            List<Cell> activeNeighbours = LevelArea.GetCellActiveNeighbors(this,result);
            Debug.Log(activeNeighbours.Count);
            return result;
        }

        private void SetCellsPositions()
        {
            _cellsSorted = _cells.OrderByDescending(x => x.transform.localPosition.y).ToList();
        }
    }
}