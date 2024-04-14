using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Components
{
    public class LineIndicator : MonoBehaviour
    {
        [SerializeField] private Image indicatorImage;
        [SerializeField] private Image thousandsImage;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private Image valueImage;
    
        private bool withTheme;

        public void HideIndicator()
        {
            indicatorImage.gameObject.SetActive(false);
            thousandsImage.gameObject.SetActive(false);
            valueText.gameObject.SetActive(false);
        }
    
        public void ShowIndicator(int value, Color color)
        {
            indicatorImage.gameObject.SetActive(true);
            thousandsImage.gameObject.SetActive(false);
            valueText.gameObject.SetActive(false);
            var index = GetValueIndex(value);
            if(index > LineManager.Instance.lineValues.Count - 1) index = LineManager.Instance.lineValues.Count - 1;
            if(withTheme) {
                valueImage.gameObject.SetActive(true);
                valueImage.sprite = SpriteManager.Instance.GetActiveThemeSprite(index);
                valueText.gameObject.SetActive(false);
            }
            else { 
                valueText.gameObject.SetActive(true);
                valueImage.gameObject.SetActive(false);
            }
            if (value > 999)
            {
                thousandsImage.gameObject.SetActive(true);
                valueText.text = (value / 1000).ToString() + "K";
            }
            else
            {
                valueText.text = value.ToString();
            }
        
            indicatorImage.color = color;
        }
        private int GetValueIndex(int value)
        {
            return LineManager.Instance.lineValues.IndexOf(value);;
        }
    }
}
