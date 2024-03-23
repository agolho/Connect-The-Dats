
    using System;
    using System.Linq;
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
    }
