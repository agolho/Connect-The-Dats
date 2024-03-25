using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [Header("Buttons")]
    public Button resetButton;
    public Button resetYesButton, resetNoButton, promptCloseButton;
    [Header("Screens")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private GameObject resetPromptScreen;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        BindButtons();
    }

    private void BindButtons()
    {
        resetButton.onClick.AddListener(ShowResetPrompt);
        resetYesButton.onClick.AddListener(GameManager.Instance.ResetAndReload);
        resetNoButton.onClick.AddListener(HideResetPrompt);
        promptCloseButton.onClick.AddListener(HideResetPrompt);
    }

    private void HideResetPrompt()
    {
        resetPromptScreen.SetActive(false);
    }

    void ShowResetPrompt()
    {
        resetPromptScreen.SetActive(true);
    }


    public void FadeIn()
    {
        fadeImage.DOFade(0, .75f);
    }
}
