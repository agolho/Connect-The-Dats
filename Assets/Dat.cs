using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dat : MonoBehaviour
{
    
    [Header("Components")]
    [SerializeField] private Image datImage;
    [SerializeField] private TextMeshProUGUI datValueText;
    
    private bool isZoomed;

    private void Start()
    {
        datImage = GetComponent<Image>();
        datValueText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetupDat(int value, Color datColor)
    {
        if (value >= 1024)
        {
            var thousands = value / 1024; 
            datValueText.text = thousands+ "K";
            datImage.color = datColor;
        }
        else
        {
            datValueText.text = value.ToString();
            datImage.color = datColor;
        }
    }
    
    public void ScaleUp()
    {
        if (isZoomed) return;
        isZoomed = true;
        if(datImage == null) datImage = GetComponent<Image>();
        datImage.rectTransform.localScale *= 1.2f;
    }

    public void ResetScale()
    {
        isZoomed = false;
        datImage.rectTransform.localScale = Vector3.one;
    }

    public void Emphasise()
    {
        ResetScale();
        datImage.rectTransform.DOComplete();
        datImage.rectTransform.DOScale(Vector3.one * 1.2f, .15f).OnComplete(() =>
        {
            datImage.rectTransform.DOScale(Vector3.one, .1f);
        });
    }
}
