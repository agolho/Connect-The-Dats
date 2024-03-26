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
            valueText.gameObject.SetActive(true);
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
    
    }
}
