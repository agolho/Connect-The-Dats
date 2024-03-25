
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using DG.Tweening;
    using UnityEngine;

    public class GridManager: MonoBehaviour
    {
        public static GridManager Instance;

        public bool IsInteractable = true;
        
        [Header("Grid Prefabs")]
        [SerializeField]
        public Dat datPrefab;
        
        [Space(10)]
        
        [Header("Grid Properties")]
        public GridCell[] grid;
        public int numRows;
        public int numCols;
        
        
        [SerializeField] private List<GridCell> emptyCells = new List<GridCell>();
        private readonly List<GridCell> _cellsToRemove = new List<GridCell>();
        private List<GridCell> _cellsToMove = new List<GridCell>();
                
        int[] _emptyCellMinRows = {5,5,5,5,5};
        int[] _emptyCellMaxRows = {0,0,0,0,0};
        int[] _moveDepth = {0,0,0,0,0};

        readonly float[] _probabilities = {0.1f, 0.224f, 0.3f, 0.175f, 0.125f, 0.05f, 0.025f, 0.001f};

        
        public static event Action MoveComplete;
        
        private void Awake()
        {
            Instance = this;
        }

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


        private GridCell GetCell(int row, int col)
        {
            return grid.FirstOrDefault(c => c.row == row && c.col == col);
        }


        public void GenerateRandomNewCell()
        {

            foreach (var cell in grid)
            {
                if (cell.cellValue != 0) continue;

                float randomNum = UnityEngine.Random.value;
                float cumulativeProbability = 0f;
                int selectedNumber = 0;

                for (int i = 0; i < _probabilities.Length; i++)
                {
                    cumulativeProbability += _probabilities[i];
                    if (randomNum <= cumulativeProbability)
                    {
                        selectedNumber = (int)Mathf.Pow(2, i + 1);
                        break;
                    }
                }

                cell.cellValue = selectedNumber;
                cell.SetUpDat();
                cell.cellDat.PopIn();
                emptyCells.Remove(cell);
            }
            OnMoveComplete();
            GameManager.Instance.SaveGame();
        }

        
        public void AddToEmptyCells(GridCell cell)
        {
            emptyCells.Add(cell);
            
            CacheMoveInformation(cell);
        }

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
            
            Invoke(nameof(GenerateRandomNewCell), 0.25f);
        }
        
        private void CacheCellsToMove()
        {
            foreach (var emptyCell in emptyCells) 
            {
                for (var row = emptyCell.row + 1; row < numRows; row++)
                {
                    if (GetCell(row, emptyCell.col).cellValue == 0) continue;
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
                    if (cellToMove.cellValue == 0) continue;
                    if (cellToMove.cellDat == null) continue;

                    var targetRow = cellToMove.row - _moveDepth[column];
                    if (targetRow < 0) _moveDepth[column] = 1;
                    var targetCell = GetCell(targetRow, column);
                    if (targetCell == null)  continue;
                    
                    if (targetCell.cellValue != 0)
                    {
                        _moveDepth[column] = 1;
                        targetRow = cellToMove.row - _moveDepth[column];
                        targetCell = GetCell(targetRow, column);
                    }
                    
                    MoveCells(cellToMove, targetCell);
                }
            }
        }

        private void MoveCells(GridCell cellToMove, GridCell targetCell)
        {
            cellToMove.cellDat.transform.DOComplete();
            cellToMove.cellDat.transform.DOMove(targetCell.transform.position, 0.1f);
            cellToMove.cellDat.SquashAndStretch();
                    
            targetCell.cellValue = cellToMove.cellValue;
            cellToMove.cellValue = 0;
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

        public int[] GetGridData()
        {
            return grid.Select(cell => cell.cellValue).ToArray();
        }

        public void LoadGrid(GridData data)
        {
            for (var i = 0; i < data.cellValues.Length; i++)
            {
                grid[i].cellValue = data.cellValues[i];
                grid[i].SetUpDat();
            }
        }

        private void OnMoveComplete()
        {
            MoveComplete?.Invoke();
        }
    }
