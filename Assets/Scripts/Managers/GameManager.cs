using System;
using Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    [Serializable]
    public class GridData
    {
        public int[] cellValues;
    }
    public class GameManager : MonoSingleton<GameManager>
    {

        [Header("Settings")] 
        public int powersOfTwoCount = 30;
    
        private void Awake()
        {
            Application.targetFrameRate = 120;
        }

        private void Start()
        {
            LoadGame();
            GridManager.Instance.SetupBoard();
        }

        #region Save and Load
        void LoadGame()
        {
            var json = PlayerPrefs.GetString("GridData");
            if (string.IsNullOrEmpty(json)) return;
            var data = JsonUtility.FromJson<GridData>(json);
            GridManager.Instance.LoadGrid(data);
        }

        public void SaveGame()
        {
            var gridData = GridManager.Instance.GetGridData();
            GridData data = new GridData();
            data.cellValues = gridData;
            var json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString("GridData", json);
        }
        #endregion

    
        public void ResetAndReload()
        {
            PlayerPrefs.DeleteKey("GridData");
            GridManager.Instance.Reset();
            SceneManager.LoadScene(0);
        }
    }
}