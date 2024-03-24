using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GridCell : MonoBehaviour
{

    public int cellValue;
    
    public bool isValid;
    public bool isStart;
    public bool isEnd;
    public bool isPath;
    
    [Header("Cell Properties")]
    public int row;
    public int col;
    public GridCell[] neighbors = new GridCell[8];
    public int index;

    public Dat cellDat;

    private TextMeshProUGUI debugText;
    public void SetUpDat()
    {
        if(cellValue == 0) return;
        isValid = true;
        var cellTransform = transform;
        if(cellDat != null) Destroy(cellDat.gameObject);
        cellDat = Instantiate(GridManager.Instance.datPrefab, cellTransform.position, cellTransform.rotation, cellTransform);
        
        //TODO: get the dat value from the grid manager
        //TODO: get the dat color from the grid manager dictionary
        cellDat.SetupDat(cellValue, LineManager.Instance.GetColorFromDictionary(cellValue));
        debugText = GetComponentInChildren<TextMeshProUGUI>();
        debugText.text = cellValue.ToString();
    }
    
    public void SetValue(int value)
    {
        cellValue = value;
    }

    private void Update()
    {
        debugText.text = cellValue+ " ["+gameObject.name +"]";
    }

    private void OnMouseDown()
    {
        if (!isValid) return;
        if(LineManager.Instance.path.Count == 0) isStart = true;
        
        
        LineManager.Instance.AddToPath(this);
        LineManager.Instance.currentLineValue = cellValue;
        cellDat.ScaleUp();
    }

    private void OnMouseEnter()
    {
        if (!isValid) return;
        if (LineManager.Instance.path.Count <= 0) return;
        if(LineManager.Instance.currentLineValue != cellValue) return;

        var lastCell = LineManager.Instance.path[^1];
        if(!CheckCellIsNeighbour(lastCell)) return;
        
        if (LineManager.Instance.path.Contains(this))
        {
            if (!CheckCellSecondFromLast(this)) return;
            
            LineManager.Instance.RemoveFromPath(lastCell);
            lastCell.cellDat.ResetScale();
            
            return;
        }
        LineManager.Instance.AddToPath(this);
        cellDat.ScaleUp();
    }

    bool CheckCellSecondFromLast(GridCell cell)
    {
        return LineManager.Instance.path.Count > 1 && LineManager.Instance.path[^2] == cell;
    }

    bool CheckCellIsNeighbour(GridCell cell)
    {
        return neighbors.Contains(cell);
    }
}
