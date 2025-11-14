using UnityEngine;

namespace CursedOnion.Game.General.UI.Canvases.Level
{
    public class InspectionWindow : MonoBehaviour
    {
        [SerializeField] private GameObject unitTurnOrder;
        [SerializeField] private GameObject unitInfo;
        
        private void Awake()
        {
            ShowUnitInfo();
        }
        
        public void ShowUnitInfo()
        {
            unitInfo.SetActive(true);
            unitTurnOrder.SetActive(false);
        }
        public void ShowUnitTurnOrder()
        {
            unitInfo.SetActive(false);
            unitTurnOrder.SetActive(true);
        }
    }
}
