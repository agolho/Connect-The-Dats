using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadGame();
        UIManager.Instance.FadeIn();
    }

    
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
    
    public void ResetAndReload()
    {
        PlayerPrefs.DeleteKey("GridData");
        SceneManager.LoadScene(0);
    }

    
}