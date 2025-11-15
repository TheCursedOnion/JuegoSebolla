using TMPro;
using UltEvents;
using UnityEngine;

namespace CursedOnion.Game.General.UI.Buttons.Extensions
{
    public class UIButtonLevelInformationExtension : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMesh;
        [SerializeField] private UltEvent OnOpen;
        [SerializeField] private UltEvent OnClose;
        
        private bool isOpen = true;

        public void Toggle()
        {
            isOpen = !isOpen;
            if (isOpen) OnOpen.Invoke();
            else OnClose.Invoke();
        }
    }
}