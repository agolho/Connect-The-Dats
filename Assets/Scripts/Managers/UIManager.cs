using DG.Tweening;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [Header("Buttons")]
        [SerializeField] private Button resetButton;
        [SerializeField] private  Button resetYesButton, resetNoButton, promptCloseButton;
        [SerializeField] private Button shuffleButton;
        
        [Header("Screens")]
        [SerializeField] private GameObject resetPromptScreen;
    
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
            shuffleButton.onClick.AddListener(ShuffleBoard);
        }

        #region Prompt

        private void HideResetPrompt()
        {
            resetPromptScreen.SetActive(false);
        }

        void ShowResetPrompt()
        {
            resetPromptScreen.SetActive(true);
        }

        #endregion

        #region Shuffle

        private void ShuffleBoard()
        {
            GridManager.Instance.ShuffleBoard();
        }
        
        public void ShuffleButtonInteractable(bool interactable)
        {
            shuffleButton.interactable = interactable;
        }
        
        public void EmphasiseShuffleButton()
        {
            shuffleButton.transform.DOComplete();
            shuffleButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 10, 1);
        }

        #endregion

    }
}
