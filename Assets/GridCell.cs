using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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

    void Subscribe()
    {
      GridManager.MoveComplete += CleanUp;   
    }
    void Unsubscribe()
    {
        GridManager.MoveComplete += CleanUp;   
    }
    private void OnDestroy()
    {
        Unsubscribe();
    }

    public void SetUpDat()
    {
        if(cellValue == 0) return;
        isValid = true;
        var cellTransform = transform;
        if(cellDat != null) Destroy(cellDat.gameObject);
        cellDat = Instantiate(GridManager.Instance.datPrefab, cellTransform.position, cellTransform.rotation, cellTransform);
        
        cellDat.SetupDat(cellValue, LineManager.Instance.GetColorFromDictionary(cellValue));
        Subscribe();
    }
    
    public void SetValue(int value)
    {
        cellValue = value;
    }

    private void OnMouseDown()
    {
        if (!isValid) return;
        if(!GridManager.Instance.IsInteractable) return;
        if(LineManager.Instance.path.Count == 0) isStart = true;
        
        LineManager.Instance.currentLineMasterValue = cellValue;
        LineManager.Instance.AddToPath(this);
        ScaleUp();
    }



    private void OnMouseEnter()
    {
        if (!isValid) return;
        if (LineManager.Instance.path.Count <= 0) return;
        if(LineManager.Instance.currentLineMasterValue != cellValue) return;

        var lastCell = LineManager.Instance.path[^1];
        if(!CheckCellIsNeighbour(lastCell)) return;
        
        if (LineManager.Instance.path.Contains(this))
        {
            if (!CheckCellSecondFromLast(this)) return;
            
            LineManager.Instance.RemoveFromPath(lastCell);
            lastCell.ResetScale();
            
            return;
        }
        LineManager.Instance.AddToPath(this);
        ScaleUp();
    }
    
    public void ScaleUp()
    {
        if (cellDat == null) return;
        cellDat.ScaleUp();
    }

    public void ResetScale()
    {
        if (cellDat == null) return;
       cellDat.ResetScale();
    }

    bool CheckCellSecondFromLast(GridCell cell)
    {
        return LineManager.Instance.path.Count > 1 && LineManager.Instance.path[^2] == cell;
    }

    bool CheckCellIsNeighbour(GridCell cell)
    {
        return neighbors.Contains(cell);
    }

    public void CleanUp()
    {
        var childDats = GetComponentsInChildren<Dat>();
        if (childDats.Length <= 1) return;
        foreach (var dat in childDats)
        {
            if (dat == cellDat) continue;
            Trash.Instance.TrashObject(dat.gameObject);
        }
    }

    public void MoveCellDatAndTrash(Vector3 transformPosition, float f)
    {
        cellDat.transform.DOMove(transformPosition, f).OnComplete(() =>
        {
            Trash.Instance.TrashObject(cellDat.gameObject);
        });
    }
}
