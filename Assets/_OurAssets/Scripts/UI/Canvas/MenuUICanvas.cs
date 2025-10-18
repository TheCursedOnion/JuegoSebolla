using CursedOnion.Game;
using CursedOnion.Helpers;
using UnityEngine;

namespace CursedOnion.UI.Canvases
{
    public class MenuUICanvas : MonoBehaviour, IUICanvas
    {
        [SerializeField] private GameObject MenuUIParent;
        
        [SerializeField] private GameObject MainScreen;
        [SerializeField] private GameObject OptionsScreen;
        [SerializeField] private GameObject CreditScreen;

        private readonly TransitionIndex transitionIndex = new TransitionIndex();
        public void SetTransitionIndex(int value) => transitionIndex.SetTransitionIndex(value);

        public bool OpenMainScreen()
        {
            return transitionIndex.IsIndexEquals(0);
        }
        public void SetMainScreen()
        {
            DisableAllScreens();
            MainScreen.SetActive(true);
        }
        
        public bool OpenOptionsScreen()
        {
            return transitionIndex.IsIndexEquals(1);
        }
        public void SetOptionsScreen()
        {
            DisableAllScreens();
            OptionsScreen.SetActive(true);
        }
        
        public bool OpenCreditsScreen()
        {
            return transitionIndex.IsIndexEquals(2);
        }
        public void SetCreditsScreen()
        {
            DisableAllScreens();
            CreditScreen.SetActive(true);
        }
        void DisableAllScreens()
        {
            MainScreen.SetActive(false);
            OptionsScreen.SetActive(false);
            CreditScreen.SetActive(false);
        }
        
    }
}
