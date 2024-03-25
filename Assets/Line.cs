using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Line : MonoBehaviour
{
    [SerializeField] private Image lineImage;
    
    public void SetColor(Color color)
    {
        lineImage.color = color;
    }
}
