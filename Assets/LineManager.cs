using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LineManager : MonoBehaviour
{
    public static LineManager Instance;
    public List<GridCell> path = new List<GridCell>();
    [SerializeField] private Line linePrefab;
    [SerializeField] private Transform lineParent;
    public int currentLineMasterValue;

    [SerializeField] private Image lineIndicatorImage;
    [SerializeField] private TextMeshProUGUI lineIndicatorText;

    [SerializeField] private Color[] valueColors;
    private readonly Dictionary<int, Color> _lineColors = new Dictionary<int, Color>();
    [SerializeField] private List<int> lineValues = new List<int>();

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
        for (var i = 1; i < GameManager.Instance.powersOfTwoCount; i++)
        {
            var colorIndex = (i - 1) % valueColors.Length;
            _lineColors.Add(
                (int)Mathf.Pow(2, i),
                valueColors[colorIndex]
            );
            lineValues.Add((int)Mathf.Pow(2, i));
        }
    }
    
    public Color GetColorFromDictionary(int value)
    {
        return _lineColors[value];
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
        UpdateLineIndicator();
        if(path.Count > 1) MergeLine();
        else CancelLine();
    }
    
    private void MergeLine()
    {
        var lineValue = CalculateLineValue();

        path[^1].transform.SetAsFirstSibling();
        for (var index = 0; index < path.Count; index++)
        {
            if (index == path.Count - 1) continue;
            var cell = path[index];
            cell.ResetScale();
            cell.SetValue(0);
            GridManager.Instance.AddToEmptyCells(cell);
            cell.MoveCellDatAndTrash(path[^1].transform.position, 0.15f);
        }
        StartCoroutine(MergeCellRoutine(lineValue, path[^1]));
        ClearPath();
        SoundManager.Instance.PlaySound(Random.Range(0, 2) == 0 ? "pop1" : "pop2");
        StartCoroutine(CellShiftRoutine());
    }

    IEnumerator MergeCellRoutine(int lineValue, GridCell mergeTargetCell)
    {
        yield return new WaitForSeconds(0.15f);
        mergeTargetCell.cellValue = lineValue;
        mergeTargetCell.cellDat.SetupDat(lineValue, GetColorFromDictionary(lineValue));
        mergeTargetCell.cellDat.Emphasise();
    }

    private int CalculateLineValue()
    {
        var lineValue = currentLineMasterValue * path.Count;

        var nearestLowerValue = lineValues[0];
        foreach (var value in lineValues)
        {
            if (value <= lineValue)
            {
                nearestLowerValue = value;
            }
            else
            {
                break;
            }
        }
    
        return nearestLowerValue;
    }

    private IEnumerator CellShiftRoutine()
    {
        yield return new WaitForSeconds(0.25f);
        GridManager.Instance.ShiftCellsDown();
    }

    private void CancelLine()
    {
        foreach (var cell in path)
        {
            cell.isStart = false;
            if(cell.cellDat != null) cell.cellDat.ResetScale();
        }

        ClearPath();
    }

    private void DrawLine(GridCell cell)
    {
        if(path.Count == 1) return;
        var direction = path[^2].transform.position - cell.transform.position;
        var linePosition = path[^2].transform.position - direction / 2;
        var lineRotation = Quaternion.FromToRotation(Vector3.right, direction);
        var line = Instantiate(linePrefab, linePosition, lineRotation, lineParent);
        line.SetColor(GetColorFromDictionary(currentLineMasterValue));
    }

    private void RemoveLastLine()
    {
        Destroy(lineParent.GetChild(lineParent.childCount - 1).gameObject);
    }
    
    void ClearLine()
    {
        foreach (Transform child in lineParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void AddToPath(GridCell cell)
    {
        path.Add(cell);
        DrawLine(cell);
        SoundManager.Instance.PlaySound("bop", path.Count);
        UpdateLineIndicator();
    }

    public void RemoveFromPath(GridCell cell)
    {
        path.Remove(cell);
        RemoveLastLine();
        SoundManager.Instance.PlaySound("bop", path.Count);
        UpdateLineIndicator();
    }
    
    public void ClearPath()
    {
        path.Clear();
        UpdateLineIndicator();
        ClearLine();
    }
    
    private void UpdateLineIndicator()
    {
        if (path.Count == 0)
        {
            lineIndicatorImage.gameObject.SetActive(false);
            return;
        }
        var value = CalculateLineValue();
        lineIndicatorText.text = value.ToString();
        lineIndicatorImage.color = GetColorFromDictionary(value);
        lineIndicatorImage.gameObject.SetActive(true);
    }
}