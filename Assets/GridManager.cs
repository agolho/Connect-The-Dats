
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
        public void AddToEmptyCells(GridCell cell)
        {
            _emptyCellsCount++;
            emptyCells.Add(cell);
            Debug.Log("added empty cell" + cell.gameObject.name);
            Invoke(nameof(ShiftCellsDown), 0.26f);
        }
        
        List<GridCell> cellsToRemove = new List<GridCell>();
        List<GridCell> cellsToMove = new List<GridCell>();
        public void ShiftCellsDown()
        {
            cellsToMove = new List<GridCell>();
            foreach (var emptyCell in emptyCells.ToList()) 
            {
                for (int row = emptyCell.row + 1; row < numCols; row++)
                {
                     cellsToMove.Add(GetCell(row, emptyCell.col));
                }
                cellsToRemove.Add(emptyCell);
            }

            for (var index = 0; index < cellsToMove.Count; index++)
            {
                for( var col = 0; col < numCols; col++)
                {
                    var cell = cellsToMove[index];
                    if(cell.col != col) continue;
                    var depth = 1;
                 
                    //depth = emptyCells.Where(c => c.col == col).Min(c => c.row) +1;
                    Debug.Log("Depth is "+ depth);
                    
                    var targetCellRow = cell.row-depth;
                    var targetCell = GetCell(targetCellRow, col);
                    if(cell.cellDat == null) continue;
                    if(cell.cellValue == 0) continue;
                    Debug.Log(cell.gameObject.name +" is moving to "+ targetCell.gameObject.name);
                    cell.cellDat.transform.DOMove(targetCell.transform.position, 0.15f)
                        .OnComplete(() =>
                        {
                            targetCell.SetValue(cell.cellValue);
                            Debug.Log("Target "+ targetCell.gameObject.name+"  value is " + targetCell.cellValue+ " and cell "+ cell.gameObject.name+" value is " + cell.cellValue) ;
                        
                            targetCell.cellDat = cell.cellDat;
                            targetCell.cellDat.transform.SetParent(targetCell.transform);
                        
                            cell.cellDat = null;
                            cell.SetValue(0);
                        
                        });
                }
            }
            foreach (var cellToRemove in cellsToRemove)
            {
                emptyCells.Remove(cellToRemove);
                _emptyCellsCount--;
            }
            cellsToMove.Clear();
            cellsToRemove.Clear();
        }
        
        IEnumerator SetEmptyCellToZero(GridCell cell)
        {
            yield return new WaitForSeconds(0.16f);
            cell.cellValue = 0;
        }
        
       
    }
