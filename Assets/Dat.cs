using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dat : MonoBehaviour
{
    
    [Header("Components")]
    [SerializeField] private Image datImage;
    [SerializeField] private TextMeshProUGUI datValueText;
    
    private bool isZoomed;

    public void SetupDat(int value, Color datColor)
    {
        datValueText.text = value.ToString();
        datImage.color = datColor;
    }
    
    
    
    public void ScaleUp()
    {
        if (isZoomed) return;
        isZoomed = true;
        datImage.rectTransform.localScale *= 1.2f;
    }

    public void ResetScale()
    {
        isZoomed = false;
        datImage.rectTransform.localScale = Vector3.one;
    }
}
