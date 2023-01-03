using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Row : MonoBehaviour
{
    [SerializeField] private List<Cell> _cells;
    private List<Vector3> _cellsPositions;

    void Start()
    {
        SetCellsPositions();
    }

    public Vector3 GetFreeCellPosition()
    {
        Vector3 result = _cellsPositions[0];
        _cellsPositions.Remove(result);
        return result;
    }

    private void SetCellsPositions()
    {
        List<Vector3> cellsPositions = new List<Vector3>();
        foreach (var cell in _cells)
        {
            cellsPositions.Add(cell.transform.position);
        }
        _cellsPositions = cellsPositions.OrderBy(x => x.y).ToList();
    }
}
