using System;
using System.Collections.Generic;
using System.Linq;
using Components;
using DG.Tweening;
using Tools;
using UnityEngine;

namespace Managers
{
    public class GridManager: MonoSingleton<GridManager>
    {
        [Header("Grid Prefabs")]
        [SerializeField]
        public Dat datPrefab;
        
        [Header("Grid Properties")]
        public GridCell[] grid;
        public int numRows;
        public int numCols;
        
        [Header("Settings")]
        [SerializeField] private float cellGenerationDelay = 0.1f;
        [SerializeField] private float cellMoveTime = 0.1f;
        
        [SerializeField] private float[] randomGenerationProbabilities = {0.1f, 0.224f, 0.3f, 0.175f, 0.125f, 0.05f, 0.025f, 0.001f};
        
        [SerializeField] private List<GridCell> emptyCells = new List<GridCell>();
        private readonly List<GridCell> _cellsToRemove = new List<GridCell>();
        private List<GridCell> _cellsToMove = new List<GridCell>();
        
        
        private readonly int[] _emptyCellMinRows = {5,5,5,5,5};
        private readonly int[] _emptyCellMaxRows = {0,0,0,0,0};
        private readonly int[] _moveDepth = {0,0,0,0,0};
        
        public static event Action MoveComplete;
        
        #region Save and Load
        public int[] GetGridData()
        {
            return grid.Select(cell => cell.GetValue()).ToArray();
        }

        public void LoadGrid(GridData data)
        {
            for (var i = 0; i < data.cellValues.Length; i++)
            {
                grid[i].SetValue(data.cellValues[i]);
                grid[i].SetUpDat();
            }
        }

        #endregion
        #region Initial Setup

        public void SetupBoard()
        {
            CalculateNeighbours();
            foreach (var cell in grid)
            {
                cell.SetUpDat();
            }
        }
        
        void CalculateNeighbours()
        {
            foreach (var cell in grid)
            {
                var row = cell.row;
                var col = cell.col;
                if (row > 0) cell.neighbors[0] = GetCell(row - 1, col); // Top neighbor
                if (row < numRows - 1) cell.neighbors[1] = GetCell(row + 1, col); // Bottom neighbor
                if (col > 0) cell.neighbors[2] = GetCell(row, col - 1); // Left neighbor
                if (col < numCols - 1) cell.neighbors[3] = GetCell(row, col + 1); // Right neighbor

                // Diagonal neighbors
                if (row > 0 && col > 0) cell.neighbors[4] = GetCell(row - 1, col - 1); // Top-left neighbor
                if (row > 0 && col < numCols - 1) cell.neighbors[5] = GetCell(row - 1, col + 1); // Top-right neighbor
                if (row < numRows - 1 && col > 0) cell.neighbors[6] = GetCell(row + 1, col - 1); // Bottom-left neighbor
                if (row < numRows - 1 && col < numCols - 1) cell.neighbors[7] = GetCell(row + 1, col + 1); // Bottom-right neighbor
            }
        }

        #endregion
        #region Cell Generation

        public void GenerateRandomNewCell()
        {

            foreach (var cell in grid)
            {
                if (cell.GetValue() != 0) continue;

                var selectedNumber = SelectANumber();

                cell.SetValue(selectedNumber);
                cell.SetUpDat();
                cell.cellDat.PopIn();
                emptyCells.Remove(cell);
            }
            OnMoveComplete();
            GameManager.Instance.SaveGame();
        }

        private int SelectANumber()
        {
            var randomNum = UnityEngine.Random.value;
            var cumulativeProbability = 0f;
            var selectedNumber = 0;

            for (var i = 0; i < randomGenerationProbabilities.Length; i++)
            {
                cumulativeProbability += randomGenerationProbabilities[i];
                if (!(randomNum <= cumulativeProbability)) continue;
                selectedNumber = (int)Mathf.Pow(2, i + 1);
                break;
            }

            return selectedNumber;
        }

        #endregion
        #region Cell Movements
        
        private void CacheMoveInformation(GridCell cell)
        {
            if(cell.row < _emptyCellMinRows[cell.col]) _emptyCellMinRows[cell.col] = cell.row;
            if(cell.row + 1 > _emptyCellMaxRows[cell.col]) _emptyCellMaxRows[cell.col] = cell.row +1;
            _moveDepth[cell.col] = _emptyCellMaxRows[cell.col] - _emptyCellMinRows[cell.col];
        }
        public void ShiftCellsDown()
        {
            _cellsToMove = new List<GridCell>();
            
            CacheCellsToMove();
            ProcessCellsToMove();
            ProcessRemoveList();
            ResetValuesAndLists();
            
            Invoke(nameof(GenerateRandomNewCell), cellGenerationDelay);
        }
        
        private void CacheCellsToMove()
        {
            foreach (var emptyCell in emptyCells) 
            {
                for (var row = emptyCell.row + 1; row < numRows; row++)
                {
                    if (GetCell(row, emptyCell.col).GetValue() == 0) continue;
                    _cellsToMove.Add(GetCell(row, emptyCell.col));
                }
            }
        }
        
        private void ProcessCellsToMove()
        {
            foreach (var cellToMove in _cellsToMove)
            {
                for (var column = 0; column < numCols; column++)
                {
                    if(cellToMove.col != column) continue;
                    
                    if (cellToMove.GetValue() == 0) continue;
                    if (cellToMove.cellDat == null) continue;

                    var targetRow = cellToMove.row - _moveDepth[column];
                    if (targetRow < 0) _moveDepth[column] = 1;
                    var targetCell = GetCell(targetRow, column);
                    
                    if (targetCell == null)  continue;
                    
                    if (targetCell.GetValue() != 0) targetCell = GetAnotherEmptyTargetCell(column, cellToMove);
                    
                    MoveCells(cellToMove, targetCell);
                }
            }
        }

        private GridCell GetAnotherEmptyTargetCell(int column, GridCell cellToMove)
        {
            // Fast edge case solution for when there are two empty cells in the same column with one non-empty cell in between
            // Not a pretty solution, given enough time I would refactor this
            
            _moveDepth[column] = 1;
            var targetRow = cellToMove.row - _moveDepth[column];
            var targetCell = GetCell(targetRow, column);
            return targetCell;
        }

        private void MoveCells(GridCell cellToMove, GridCell targetCell)
        {
            cellToMove.cellDat.transform.DOComplete();
            cellToMove.cellDat.transform.DOMove(targetCell.transform.position, cellMoveTime);
            cellToMove.cellDat.SquashAndStretch();
                    
            targetCell.SetValue(cellToMove.GetValue());
            cellToMove.SetValue(0);
            targetCell.cellDat = cellToMove.cellDat;
            cellToMove.cellDat.transform.SetParent(targetCell.transform);
            cellToMove.cellDat = null;
            _cellsToRemove.Add(targetCell);
        }

        private void ProcessRemoveList()
        {
            foreach (var cellToRemove in _cellsToRemove)
            {
                emptyCells.Remove(cellToRemove);
            }
        }
        
        private void ResetValuesAndLists()
        {
            _cellsToMove.Clear();
            _cellsToRemove.Clear();
            
            ResetMoveValues();
        }
        
        void ResetMoveValues()
        {
            for (var i = 0; i < numCols; i++)
            {
                _emptyCellMinRows[i] = 5;
                _emptyCellMaxRows[i] = 0;
                _moveDepth[i] = 0;
            }
        }

        #endregion
        #region Functions

        private GridCell GetCell(int row, int col)
        {
            return grid.FirstOrDefault(c => c.row == row && c.col == col);
        }

        public void AddToEmptyCells(GridCell cell)
        {
            emptyCells.Add(cell);
            
            CacheMoveInformation(cell);
        }
        
        public void Reset()
        {
            foreach (var cell in grid)
            {
                cell.Unsubscribe();
            }
        }

        #endregion
        
        private void OnMoveComplete()
        {
            MoveComplete?.Invoke();
        }


    }
}
