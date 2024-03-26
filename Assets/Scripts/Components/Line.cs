using UnityEngine;
using UnityEngine.UI;

namespace Components
{
    public class Line : MonoBehaviour
    {
        [SerializeField] private Image lineImage;
    
        public void SetColor(Color color)
        {
            lineImage.color = color;
        }
    }
}
