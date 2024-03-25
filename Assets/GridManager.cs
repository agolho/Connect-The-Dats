
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
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            CalculateNeighbours();
            SetupBoard();
        }

        void SetupBoard()
        {
            foreach (var cell in grid)
            {
                cell.SetUpDat();
            }
        }
        
        public void SetInteractablilty(bool value)
        {
            IsInteractable = value;
            if(value) GameManager.Instance.SaveGame();
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
                
                cell.cellValue = (int)Mathf.Pow(2, UnityEngine.Random.Range(1, 5));
                cell.SetUpDat();
                emptyCells.Remove(cell);
            }
            SetInteractablilty(true);
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
            foreach (var ctm in _cellsToMove)
            {
                for (int column = 0; column < numCols; column++)
                {
                    if(ctm.col != column) continue;
                    var targetCell = GetCell(ctm.row - _moveDepth[column], column);
                    if (ctm.cellValue == 0) continue;
                    if (ctm.cellDat == null) continue;
                    
                    //Debug.Log(ctm.gameObject.name + " is moving to " + targetCell.gameObject.name);

                    ctm.cellDat.transform.DOMove(targetCell.transform.position, 0.25f);
                    
                    targetCell.cellDat = ctm.cellDat;
                    ctm.cellDat.transform.SetParent(targetCell.transform);
                    ctm.cellDat = null;
                    targetCell.cellValue = ctm.cellValue;
                    ctm.cellValue = 0;
                    _cellsToRemove.Add(targetCell);
                }
            }
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
    }
