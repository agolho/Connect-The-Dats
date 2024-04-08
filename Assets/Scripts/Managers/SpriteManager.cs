using System.Collections;
using System.Collections.Generic;
using Managers;
using Tools;
using UnityEngine;

public class SpriteManager : MonoSingleton<SpriteManager>
{
    [SerializeField] private ThemeData[] themes;
    
    
    public Sprite GetActiveThemeSprite(int value)
    {
        if(value >= themes[GameManager.Instance.ActiveThemeIndex].valueSprites.Length)
        {
            return themes[GameManager.Instance.ActiveThemeIndex].valueSprites[^1];
        }
        return themes[GameManager.Instance.ActiveThemeIndex].valueSprites[value];
    }
    public Sprite GetSprite(int themeIndex, int value)
    {
        if(value >= themes[themeIndex].valueSprites.Length)
        {
            return themes[themeIndex].valueSprites[^1];
        }
        return themes[themeIndex].valueSprites[value];
    }
}
