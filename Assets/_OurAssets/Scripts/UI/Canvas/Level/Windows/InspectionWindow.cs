using UnityEngine;

namespace CursedOnion.UI.Canvases.Level
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
            unitTurnOrder.SetActive(true);
            unitInfo.SetActive(true);
        }
        public void ShowUnitTurnOrder()
        {
            unitInfo.SetActive(false);
            unitTurnOrder.SetActive(true);
        }
    }
}
