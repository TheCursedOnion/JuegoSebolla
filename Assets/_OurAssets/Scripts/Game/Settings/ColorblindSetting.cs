using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace CursedOnion.Game.Settings
{
    [Serializable]
    public class ColorblindSetting : IGlobalVolumeSetting<ColorblindSetting>
    {
        public enum ColorblindMode
        {
            Normal = 0,
            Protanopia = 1,
            Deuteranopia = 2,
            Tritanopia = 3,
        }
        
        [SerializeField] Texture2D[] LUT_s;
        public int currentLUT;
        
        public GlobalVolume GlobalVolume { get; set; }

        public void SetGlobalVolume(GlobalVolume volume)
        {
            GlobalVolume = volume;
        }
        public Texture2D GetCurrentLUT()
        {
            if(currentLUT == 0) return null;

            return LUT_s[currentLUT-1]; 
        }
        public ColorblindMode CurrentMode => (ColorblindMode)currentLUT;
        
        public Action<ColorblindSetting> OnChange { get; set; }
        
        public void MoveColorblindMode(int offset)
        {
            int index = (currentLUT + offset) % (LUT_s.Length + 1);
            SetColorblindMode(index);
        }
        void SetColorblindMode(int mode)
        {
            int index = mode;
            if(currentLUT.Equals(index)) return;
            
            currentLUT = index;
            
            if (GlobalVolume.GetVolume().profile.TryGet(out ColorLookup colorLookup))
                colorLookup.texture.value = GetCurrentLUT();
            
            OnChange?.Invoke(this);
        }
    }
}