using UnityEngine;
using System.Runtime.InteropServices;
using CursedOnion.Locators;
using Reflex.Attributes;
using TMPro;

namespace CursedOnion.Game.Inputs
{
    public class DeviceDetector : MonoBehaviour
    {
        [Inject] RuntimeVariableLocator variableLocator;
        
        #if !UNITY_EDITOR && UNITY_WEBGL
            [System.Runtime.InteropServices.DllImport("__Internal")]
            private static extern bool IsMobile();
        #endif
        private void CheckIfMobile()
        {
            #if !UNITY_EDITOR && UNITY_WEBGL
                variableLocator.IsGamePlayedOnMobile = IsMobile();
            #else
                variableLocator.IsGamePlayedOnMobile = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
            #endif
            
            if (variableLocator.IsGamePlayedOnMobile)
                Debug.Log("✅ Móvil o tablet detectado");
            else
                Debug.Log("💻 PC detectado");
        }

        void Start()
        {
            CheckIfMobile();
        }
    }
}