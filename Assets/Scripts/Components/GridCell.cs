using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using Managers;
using UnityEngine;

namespace Components
{
    [System.Serializable]
    public class GridCell : MonoBehaviour
    {

        [SerializeField] private int cellValue;
    
        public bool isValid;
        public bool setToMove;
        public bool isClaimed;
        public GridCell claimedBy;
        
        [Header("Cell Properties")]
        public int row;
        public int col;
        public GridCell[] neighbors = new GridCell[8];
        public int index;

        public Dat cellDat;
    
        

        #region Setup

        private void Start()
        {
            Subscribe();
        }

        public void SetUpDat()
        {
            if(cellValue == 0) return;
            isValid = true;
            var cellTransform = transform;
            if(cellDat != null) Destroy(cellDat.gameObject);
            cellDat = Instantiate(GridManager.Instance.datPrefab, cellTransform.position, cellTransform.rotation, cellTransform);
        
            cellDat.SetupDat(cellValue, LineManager.Instance.GetColorFromDictionary(cellValue));
        }
        public void SetupDat(int value, Color color)
        {
            cellDat.SetupDat(value, color);
        }

        #endregion
        #region Cell Operations

        public void SetValue(int value)
        {
            cellValue = value;
        }
        public int GetValue()
        {
            return cellValue;
        }

        public void SetToMove()
        {
            if (cellValue == 0)
            {
                TopNeighbourSetMove();
                return;
            }
            setToMove = true;
        }
        
        bool CheckCellSecondFromLast(GridCell cell)
        {
            return LineManager.Instance.path.Count > 1 && LineManager.Instance.path[^2] == cell;
        }

        bool CheckCellIsNeighbour(GridCell cell)
        {
            return neighbors.Contains(cell);
        }

        bool CheckCellIsEmpty()
        {
            if(cellValue == 0) return true;
            return false;
        }
        

        public void CleanUp()
        {
            var childDats = GetComponentsInChildren<Dat>();
            if (childDats.Length <= 1) return;
            foreach (var dat in childDats)
            {
                if (dat == cellDat) continue;
                Trash.Instance.TrashObject(dat.gameObject);
            }
        }

        #endregion
        #region Movement
        
        GridCell target = null;
        private void ResearchMove()
        {
            if (!setToMove) return;
            
            // if there is a lower cell set to move, wait for it to finish
            for (int i = 0; i < row; i++)
            {
                var cell = GridManager.Instance.GetCell(i, col);
                if (cell.setToMove) return;
            }
            
            // search down in the cell column and claim the lowest unclaimed cell
            for (var i = 0; i < row; i++)
            {
                var cell = GridManager.Instance.GetCell(i, col);
                if (cell.isClaimed) continue;
                if(cell.GetValue() != 0) continue;
                
                ClaimAndSetCell(cell);
                
                break;
            }
            MoveDown();
            TopNeighbourSetMove();
        }

        private void ClaimAndSetCell(GridCell cell)
        {
            cell.isClaimed = true;
            cell.claimedBy = this;
            target = cell;
        }

        private void TopNeighbourSetMove()
        {
            if(row == GridManager.Instance.numRows - 1) return;
            
            var topNeighbour = GridManager.Instance.GetCell(row + 1, col);
            if (topNeighbour == null) return;
            
            topNeighbour.SetToMove();
            topNeighbour.ResearchMove();
        }

        void MoveDown()
        {
            if (target == null) return;
            MoveCellDat(target.transform.position,.1f);
            target.SetValue(cellValue);
            cellValue = 0;
            target.cellDat = cellDat;
            target.cellDat.transform.SetParent(target.transform);
            target.SquashAndStretch();
            cellDat = null;
            setToMove = false;
            target.isClaimed = false;
            target.claimedBy = null;
        }

        public void MoveCellDat(Vector3 transformPosition, float time)
        {
            cellDat.transform.DOComplete();
            cellDat.transform.DOMove(transformPosition, time);
        }

        #endregion
        #region Input Handling
        private void OnMouseDown()
        {
            if (!isValid) return;
            LineManager.Instance.currentLineMasterValue = cellValue;
            LineManager.Instance.AddToPath(this);
            ScaleUp();
        }

        private void OnMouseEnter()
        {
            if (!isValid) return;
            if (LineManager.Instance.path.Count <= 0) return;
            if(LineManager.Instance.currentLineMasterValue != cellValue) return;

            var lastCell = LineManager.Instance.path[^1];
            if(!CheckCellIsNeighbour(lastCell)) return;
        
            if (LineManager.Instance.path.Contains(this))
            {
                if (!CheckCellSecondFromLast(this)) return;
            
                LineManager.Instance.RemoveFromPath(lastCell);
                lastCell.ResetScale();
            
                return;
            }
            LineManager.Instance.AddToPath(this);
            ScaleUp();
        }
        #endregion
        #region Dot Animations

        public void ScaleUp()
        {
            if (cellDat == null) return;
            cellDat.ScaleUp();
        }

        public void ResetScale()
        {
            if (cellDat == null) return;
            cellDat.ResetScale();
        }
    
        public void Emphasise()
        {
            if (cellDat == null) return;
            cellDat.Emphasise();
        }
    
        public void PopIn()
        {
            if (cellDat == null) return;
            cellDat.PopIn();
        }
    
        public void SquashAndStretch()
        {
            cellDat.SquashAndStretch();
        }

        #endregion
        #region Event Subscriptions
    
        void Subscribe()
        {
            LineManager.MergeComplete += ResearchMove;
        }
        public void Unsubscribe()
        {
            LineManager.MergeComplete -= ResearchMove;
        }
        private void OnDestroy()
        {
            Unsubscribe();
        }

        #endregion
    }
}
