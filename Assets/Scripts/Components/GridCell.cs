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
    
        [Header("Cell Properties")]
        public int row;
        public int col;
        public GridCell[] neighbors = new GridCell[8];
        public int index;

        public Dat cellDat;


        #region Setup
        public void SetUpDat()
        {
            if(cellValue == 0) return;
            isValid = true;
            var cellTransform = transform;
            if(cellDat != null) Destroy(cellDat.gameObject);
            cellDat = Instantiate(GridManager.Instance.datPrefab, cellTransform.position, cellTransform.rotation, cellTransform);
        
            cellDat.SetupDat(cellValue, LineManager.Instance.GetColorFromDictionary(cellValue));
            Subscribe();
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
    
        bool CheckCellSecondFromLast(GridCell cell)
        {
            return LineManager.Instance.path.Count > 1 && LineManager.Instance.path[^2] == cell;
        }

        bool CheckCellIsNeighbour(GridCell cell)
        {
            return neighbors.Contains(cell);
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
        #region Movement

        public void MoveCellDat(Vector3 transformPosition, float time)
        {
            cellDat.transform.DOComplete();
            cellDat.transform.DOMove(transformPosition, time);
        }
    
        public void MoveCellDatAndTrash(Vector3 transformPosition, float time)
        {
            cellDat.transform.DOComplete();
            cellDat.transform.DOMove(transformPosition, time).OnComplete(() =>
            {
                Trash.Instance.TrashObject(cellDat.gameObject);
            });
        }

        #endregion
        #region Event Subscriptions
    
        void Subscribe()
        {
            GridManager.MoveComplete += CleanUp;   
        }
        void Unsubscribe()
        {
            GridManager.MoveComplete += CleanUp;   
        }
        private void OnDestroy()
        {
            Unsubscribe();
        }

        #endregion
    }
}
