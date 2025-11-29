using UnityEngine;

namespace CursedOnion.Game.General.UI.Canvases.Level
{
    public class InspectionWindow : MonoBehaviour
    {
        [SerializeField] private GameObject unitInfo;
        
        private void Awake()
        {
            ShowUnitInfo();
        }
        
        public void ShowUnitInfo()
        {
            unitInfo.SetActive(true);
        }
    }
}
