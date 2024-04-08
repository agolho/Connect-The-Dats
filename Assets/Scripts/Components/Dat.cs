using System.Collections;
using DG.Tweening;
using Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Components
{
    public class Dat : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Image datImage;
        [SerializeField] private Image thousandsImage;
        [SerializeField] private Image valueImage;
        [SerializeField] private TextMeshProUGUI datValueText;
    
        private bool _isZoomed;

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
                //datValueText.text = thousands+ "K";
                datImage.color = datColor;
                thousandsImage.gameObject.SetActive(true);
            }
            else
            {
                //datValueText.text = value.ToString();
                datImage.color = datColor;
                thousandsImage.gameObject.SetActive(false);
            }
            var index = GetValueIndex(value);
            if(index > LineManager.Instance.lineValues.Count - 1) index = LineManager.Instance.lineValues.Count - 1;
            valueImage.sprite = SpriteManager.Instance.GetActiveThemeSprite(index);
        }

        private int GetValueIndex(int value)
        {
            return LineManager.Instance.lineValues.IndexOf(value);;
        }

        #region Animations

        public void ScaleUp()
        {
            if (_isZoomed) return;
            _isZoomed = true;
            if(datImage == null) datImage = GetComponent<Image>();
            datImage.rectTransform.localScale *= 1.1f;
        }

        public void ResetScale()
        {
            _isZoomed = false;
            datImage.rectTransform.localScale = Vector3.one;
        }

        public void Emphasise()
        {
            ResetScale();
            datImage.rectTransform.DOComplete();
            datImage.rectTransform.DOScale(Vector3.one * 1.3f, .15f).OnComplete(() =>
            {
                datImage.rectTransform.DOScale(Vector3.one, .1f);
            });
        }

        public void PopIn()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, .2f).SetEase(Ease.OutBack);
        }
        
        public void PopOut()
        {
            transform.DOScale(Vector3.zero, .2f).SetEase(Ease.InBack);
        }
    
        public void SquashAndStretch()
        {
            if (!gameObject.activeInHierarchy) return;
            StartCoroutine(SquashAndStretchRoutine());
        }
        private IEnumerator SquashAndStretchRoutine()
        {
            
            yield return new WaitForSeconds(0.1f);
            transform.DOLocalMove(Vector3.up * -.2f, .07f)
                .OnComplete(() =>
                {
                    transform.DOLocalMove(Vector3.zero, .1f);
                });
            transform.DOScale(Vector3.up * .8f + Vector3.right, .07f)
                .OnComplete(() =>
                {
                    transform.DOScale(Vector3.one, .1f);
                });
        }
        #endregion
    
   
    }
}
