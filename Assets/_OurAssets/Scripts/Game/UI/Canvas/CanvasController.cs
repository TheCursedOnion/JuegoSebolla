using CursedOnion.Helpers;
using UnityEngine;

namespace CursedOnion.Game.UI
{
    public class CanvasController : MonoBehaviour
    {
        [SerializeField] private MenuUIController menuUIController;
        [SerializeField] private BattleUIController battleUIController;
        
        private readonly TransitionIndex transitionIndex = new TransitionIndex();
        public void SetTransitionIndex(int value) => transitionIndex.SetTransitionIndex(value);

        void DisableAllUIs()
        {
            menuUIController.SetEnabled(false);
            battleUIController.SetEnabled(false);
        }

        public bool ChangeUI()
        {
            return transitionIndex.IsIndexEquals(0);
        }
        
        public void EnableOnlyMenuUI()
        {
            DisableAllUIs();
            menuUIController.SetEnabled(true);
        }
        public void EnableOnlyBattleUI()
        {
            DisableAllUIs();
            battleUIController.SetEnabled(true);
        }
    }
}