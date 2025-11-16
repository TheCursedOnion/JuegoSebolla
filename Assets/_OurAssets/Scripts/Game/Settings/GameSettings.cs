using UnityEngine;

namespace CursedOnion.Game.Settings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Settings/Scriptable Settings")]
    public class GameSettings : ScriptableObject
    {
        [System.NonSerialized] GlobalVolume globalVolume;
        public GlobalVolume GetGlobalVolume() => globalVolume;
        public void SetGlobalVolume(GlobalVolume globalVolume)
        {
            this.globalVolume = globalVolume;
            ColorblindSettings.SetGlobalVolume(globalVolume);
        }

        public void Initialize()
        {
            ColorblindSettings.SetColorblindMode(ColorblindSetting.ColorblindMode.Normal);
            LanguageSettings.SetUsedLanguage(LanguageSetting.Language.Spanish);
            
            LanguageSettings.Initialize();
            VolumeSettings.Initialize();
        }
        
        public VolumeSetting VolumeSettings;
        public DeviceSetting DeviceSettings;
        public LanguageSetting LanguageSettings;
        public ColorblindSetting ColorblindSettings;
    }
}
