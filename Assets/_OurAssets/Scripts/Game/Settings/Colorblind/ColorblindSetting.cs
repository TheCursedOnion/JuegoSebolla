using System;
using CursedOnion.Game.CloudSave;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace CursedOnion.Game.Settings
{
    [Serializable]
    public class ColorblindSetting : IGlobalVolumeSetting<ColorblindSetting.ColorblindMode>, IGlobalVolumeSetting, ICloudStorable
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
        public ColorblindMode GetCurrentColorblindMode() => (ColorblindMode)currentLUT;
        
        public Action<ColorblindMode> OnChange { get; set; }
        
        public void MoveColorblindMode(int offset)
        {
            int length = LUT_s.Length + 1;
            int index = (((currentLUT + offset) % length) + length) % length;
            SetColorblindMode(index);
        }
        public void SetColorblindMode(ColorblindMode mode)
        {
            SetColorblindMode((int)mode);
        }
        void SetColorblindMode(int mode)
        {
            int index = mode;
            if(currentLUT.Equals(index)) return;
            
            currentLUT = index;
            
            if (GlobalVolume == null) return;
            
            if (GlobalVolume.GetVolume().profile.TryGet(out ColorLookup colorLookup))
                colorLookup.texture.value = GetCurrentLUT();
            
            OnChange?.Invoke((ColorblindMode)currentLUT);
        }
        
        #region Cloud Storing
        public CloudSaveClient SaveClient { get; set; }
        public async void Save()
        {
            try
            {
                await SaveClient.Save("colorblind", currentLUT);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error al guardar Colorblind Setting: " + e);
            }
        }
        public async void Load()
        {
            try
            {
                var usedLut = await SaveClient.Load<int>("colorblind");
                SetColorblindMode(usedLut);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error al guardar Colorblind Setting: " + e);
            }
        }
        #endregion
    }
}