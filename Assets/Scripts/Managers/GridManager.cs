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
        public float cellGenerationDelay = 0.1f;
        public float cellMoveTime = 0.1f;
        
        // Each index represents the probability of generating a number with 2^(index+1)
        // Total probability should be 1
        [SerializeField] private float[] randomGenerationProbabilities = {0.1f, 0.224f, 0.3f, 0.175f, 0.125f, 0.05f, 0.025f, 0.001f};
        
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
            }
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
        #region Functions

        public GridCell GetCell(int row, int col)
        {
            return grid.FirstOrDefault(c => c.row == row && c.col == col);
        }

        public void SetTopNeighbourToMove(GridCell cell)
        {
            GetCell(cell.row+1, cell.col)?.SetToMove();
        }
        
        public void Reset()
        {
            foreach (var cell in grid)
            {
                cell.Unsubscribe();
            }
        }

        #endregion
    }
}
