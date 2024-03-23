using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    public static LineManager Instance;
    public List<GridCell> path = new List<GridCell>();
    public int currentLineValue;
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            MouseUp();
        }
    }

    private void MouseUp()
    {
        if(path.Count > 1) MergeLine();
        else CancelLine();
    }
    
    private void MergeLine()
    {
        var lineValue = currentLineValue * path.Count;

        for (var index = 0; index < path.Count; index++)
        {
            if(index == path.Count - 1) continue;
            var cell = path[index];
            cell.cellDat.ResetScale();
            cell.cellValue = 0;
            cell.cellDat.transform.DOMove(path[^1].transform.position, 0.25f).OnComplete(() =>
            {
                Destroy(cell.cellDat.gameObject);
            });
        }
        path[^1].cellValue = lineValue;
        path[^1].cellDat.SetupDat(lineValue, Color.red);
        path[^1].cellDat.ResetScale();
        ClearPath();
    }

    private void CancelLine()
    {
        foreach (var cell in path)
        {
            cell.isStart = false;
            cell.cellDat.ResetScale();
        }

        ClearPath();
    }

    public void AddToPath(GridCell cell)
    {
        path.Add(cell);
    }
    
    public void RemoveFromPath(GridCell cell)
    {
        path.Remove(cell);
    }
    
    public void ClearPath()
    {
        path.Clear();
    }
}