using UnityEngine;
using System.Runtime.InteropServices;
using TMPro;

namespace CursedOnion.Game.Inputs
{
    public class DeviceDetector : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI textMesh;
        #if !UNITY_EDITOR && UNITY_WEBGL
            [System.Runtime.InteropServices.DllImport("__Internal")]
            private static extern bool IsMobile();
        #endif
        private void CheckIfMobile()
        {
            bool isMobile = false;

            #if !UNITY_EDITOR && UNITY_WEBGL
                isMobile = IsMobile();
            #else
                isMobile = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
            #endif

            textMesh.text = isMobile ? "Mobile" : "PC";
            
            if (isMobile)
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