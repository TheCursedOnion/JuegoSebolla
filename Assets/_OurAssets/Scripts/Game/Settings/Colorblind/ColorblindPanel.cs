using System;
using Reflex.Attributes;
using Reflex.Extensions;
using TMPro;
using UnityEngine;

namespace CursedOnion.Game.Settings
{
    public class ColorblindPanel : MonoBehaviour
    {
        ColorblindSetting colorblindSetting;
        
        [SerializeField] private TextMeshProUGUI colorblindModeText;
        private void OnEnable()
        {
            colorblindSetting ??= gameObject.scene.GetSceneContainer().Resolve<GameSettings>().ColorblindSettings;
            colorblindSetting.OnChange += UpdateText;
            UpdateText(colorblindSetting);
        }
        private void OnDisable()
        {
            if(colorblindSetting == null) return;
            
            colorblindSetting.OnChange -= UpdateText;
        }

        public void NextColorblindMode()
        {
            colorblindSetting.MoveColorblindMode(1);
        }
        public void PreviousColorblindMode()
        {
            colorblindSetting.MoveColorblindMode(-1);
        }
        void UpdateText(ColorblindSetting setting)
        {
            colorblindModeText.text = colorblindSetting.CurrentMode.ToString();
        }
        
    }
}
