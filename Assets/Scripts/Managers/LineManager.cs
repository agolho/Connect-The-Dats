using System.Collections;
using System.Collections.Generic;
using Components;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Managers
{
    public class LineManager : MonoSingleton<LineManager>
    {
        public List<GridCell> path = new List<GridCell>();
        public int currentLineMasterValue;

        [Header("Components")]
        [SerializeField] private Line linePrefab;
        [SerializeField] private Transform lineParent;
        [SerializeField] private LineIndicator lineIndicator;
        [Header("Values")]
        [SerializeField] private Color[] valueColors;
        [SerializeField] private List<int> lineValues = new List<int>();
        
        
        private readonly Dictionary<int, Color> _lineColors = new Dictionary<int, Color>();
        
        private void Start()
        {
            BuildDictionary();
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

        #region Setup
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
        #endregion
        #region Merging Dots
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
            mergeTargetCell.SetValue(lineValue);
            mergeTargetCell.SetupDat(lineValue, GetColorFromDictionary(lineValue));
            mergeTargetCell.Emphasise();
        }
    
        private IEnumerator CellShiftRoutine()
        {
            yield return new WaitForSeconds(0.25f);
            GridManager.Instance.ShiftCellsDown();
        }

        #endregion
        #region Line Operations

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

        private void CancelLine()
        {
            foreach (var cell in path)
            {
                cell.ResetScale();
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

        #endregion
        #region Path Operations
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
                lineIndicator.HideIndicator();
                return;
            }
            var value = CalculateLineValue();
            lineIndicator.ShowIndicator(value, GetColorFromDictionary(value));
        }
        #endregion

    

    }
}