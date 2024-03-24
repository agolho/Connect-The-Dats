
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using DG.Tweening;
    using UnityEngine;

    public class GridManager: MonoBehaviour
    {
        public static GridManager Instance;

        [Header("Grid Prefabs")]
        [SerializeField]
        public Dat datPrefab;
        
        [Space(10)]
        
        [Header("Grid Properties")]
        public GridCell[] grid;
        public int numRows;
        public int numCols;
        
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
                Debug.Log(cell.cellValue);
                if (cell.cellValue != 0) continue;
                Debug.Log("Generating new cell");
                
                cell.cellValue = (int)Mathf.Pow(2, UnityEngine.Random.Range(1, 3));
                cell.SetUpDat();
            }
        }
        
        private bool CheckEmptyCells()
        {
            foreach (var cell in grid)
            {
                if (cell.cellValue != 0) continue;
                emptyCells.Add(cell);
                return true;
            }
            return false;
        }

        private int _emptyCellsCount;
        [SerializeField] private List<GridCell> emptyCells = new List<GridCell>();
        List<GridCell> cellsToRemove = new List<GridCell>();
        List<GridCell> cellsToMove = new List<GridCell>();
                
        int[] _emptyCellMinRows = {5,5,5,5,5};
        int[] _emptyCellMaxRows = {0,0,0,0,0};
        int[] _moveDepth = {0,0,0,0,0};
        
        void ResetMoveValues()
        {
            for (var i = 0; i < numCols; i++)
            {
                _emptyCellMinRows[i] = 5;
                _emptyCellMaxRows[i] = 0;
                _moveDepth[i] = 0;
            }
        }
        
        public void AddToEmptyCells(GridCell cell)
        {
            _emptyCellsCount++;
            emptyCells.Add(cell);
            
            if(cell.row < _emptyCellMinRows[cell.col]) _emptyCellMinRows[cell.col] = cell.row;
            if(cell.row + 1 > _emptyCellMaxRows[cell.col]) _emptyCellMaxRows[cell.col] = cell.row +1;
            _moveDepth[cell.col] = _emptyCellMaxRows[cell.col] - _emptyCellMinRows[cell.col];
            
            Debug.Log("added empty cell" + cell.gameObject.name);
        }
        
        public void ShiftCellsDown()
        {
            cellsToMove = new List<GridCell>();
            foreach (var emptyCell in emptyCells) 
            {
                for (var row = emptyCell.row + 1; row < numRows; row++)
                {
                    if (GetCell(row, emptyCell.col).cellValue == 0) continue;
                    cellsToMove.Add(GetCell(row, emptyCell.col));
                }
            }
            
            // for (var column = 0; column < numCols; column++)
            // {
            //     foreach (var ctm in cellsToMove)
            //     {
            //         if (ctm.col != column) continue;
            //         var targetCell = GetCell(ctm.row - _moveDepth[column] , column) ;
            //         ctm.cellDat.transform.DOMove(targetCell.transform.position, 0.25f);
            //     }
            // }
            
            foreach (var ctm in cellsToMove)
            {
                for (int column = 0; column < numCols; column++)
                {
                    if(ctm.col != column) continue;
                    var targetCell = GetCell(ctm.row - _moveDepth[column], column);
                    if (ctm.cellValue == 0) continue;
                    if (ctm.cellDat == null) continue;
                    
                    Debug.Log(ctm.gameObject.name + " is moving to " + targetCell.gameObject.name);

                    ctm.cellDat.transform.DOMove(targetCell.transform.position, 0.25f).OnComplete(() =>
                    {
                        Debug.Log("Target "+ targetCell.gameObject.name+"  value is " + targetCell.cellValue+ " and cell "+ ctm.gameObject.name+" value is " + ctm.cellValue) ;
                    });
                    targetCell.cellDat = ctm.cellDat;
                    ctm.cellDat.transform.SetParent(targetCell.transform);
                    ctm.cellDat = null;
                    targetCell.cellValue = ctm.cellValue;
                    ctm.cellValue = 0;
                    cellsToRemove.Add(targetCell);
                }
            }
            
            foreach (var cellToRemove in cellsToRemove)
            {
                emptyCells.Remove(cellToRemove);
                _emptyCellsCount--;
            }
            cellsToMove.Clear();
            cellsToRemove.Clear();
            ResetMoveValues();
        }
        
        IEnumerator SetEmptyCellToZero(GridCell cell)
        {
            yield return new WaitForSeconds(0.16f);
            cell.cellValue = 0;
        }
        
       
    }
