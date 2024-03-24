using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    public static LineManager Instance;
    public List<GridCell> path = new List<GridCell>();
    public int currentLineValue;


    [SerializeField] private Color[] valueColors;
    public Dictionary<int, Color> LineColors = new Dictionary<int, Color>();
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        BuildDictionary();
    }

    void BuildDictionary()
    {
        for (var i = 1; i < 7; i++)
        {
            LineColors.Add(
                (int) Mathf.Pow(2,i),
                valueColors[i-1]
                );
        }
    }
    
    public Color GetColorFromDictionary(int value)
    {
        return LineColors[value];
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
        // allow only mathematical powers of 2
        
        var validOddLineCount = path.Count % 2 == 0 ? path.Count : path.Count - 1;
        var lineValue = currentLineValue * validOddLineCount;

        path[^1].transform.SetAsLastSibling();
        for (var index = 0; index < path.Count; index++)
        {
            if (index == path.Count - 1) continue;
            var cell = path[index];
            cell.cellDat.ResetScale();
            cell.cellValue = 0;
            GridManager.Instance.AddToEmptyCells(cell);
            cell.cellDat.transform.DOMove(path[^1].transform.position, 0.25f).OnComplete(() =>
            {
                Destroy(cell.cellDat.gameObject);
            });
        }
        path[^1].cellValue = lineValue;
        path[^1].cellDat.SetupDat(lineValue, GetColorFromDictionary(lineValue));
        path[^1].cellDat.ResetScale();
        ClearPath();
        
        StartCoroutine(CellShiftRoutine());
    }

    private IEnumerator CellShiftRoutine()
    {
        yield return new WaitForSeconds(0.15f);
        //GridManager.Instance.ShiftCellsDown();
        yield return new WaitForSeconds(0.15f);
        //GridManager.Instance.CheckEmptyCells();
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