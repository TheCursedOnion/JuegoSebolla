using UnityEngine;

namespace CursedOnion.Game.UI.Canvas
{
    public class MenuCanvasController : MonoBehaviour
    {
        [SerializeField] private GameObject MainScreen;
        [SerializeField] private GameObject OptionsScreen;
        [SerializeField] private GameObject CreditScreen;

        [SerializeField] private int transitionIndex = -1;
        void ResetTransitionIndex() => transitionIndex = -1;
        public void SetTransitionIndex(int index) => transitionIndex = index;
        
        public bool OpenMainScreen() => transitionIndex == 0;
        public void SetMainScreen()
        {
            DisableAllScreens();
            MainScreen.SetActive(true);
        }
        
        public bool OpenOptionsScreen() => transitionIndex == 1;
        public void SetOptionsScreen()
        {
            DisableAllScreens();
            OptionsScreen.SetActive(true);
            ResetTransitionIndex();
        }
        
        public bool OpenCreditsScreen() => transitionIndex == 2;
        public void SetCreditsScreen()
        {
            DisableAllScreens();
            CreditScreen.SetActive(true);
            ResetTransitionIndex();
        }
        void DisableAllScreens()
        {
            MainScreen.SetActive(false);
            OptionsScreen.SetActive(false);
            CreditScreen.SetActive(false);
            ResetTransitionIndex();
        }
        
    }
}
