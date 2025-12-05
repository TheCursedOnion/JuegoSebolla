using System;
using CursedOnion.Locators;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Modes.Menu.UI
{
    public class PaperControls : MonoBehaviour
    {
        [Inject] RuntimeVariableLocator variableLocator;

        [SerializeField] private GameObject pcControls;
        [SerializeField] private GameObject mobileControls;
        private void Start()
        {
            bool isMobile = variableLocator.IsGamePlayedOnMobile;
            
            pcControls.SetActive(!isMobile);
            mobileControls.SetActive(isMobile);
        }
    }
}
