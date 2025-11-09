using System;
using UnityEngine;

namespace CursedOnion.Game.Settings
{
    [Serializable]
    public class LanguageSetting : ISetting<LanguageSetting>
    {
        public enum Language
        {
            Spanish = 0,
            English = 1
        }
        [SerializeField] Language currentLanguage;
        public Language CurrentLanguage => currentLanguage;
        public Action<LanguageSetting> OnChange { get; set; }
        
        public void MoveLanguage(int offset)
        {
            int current = (int)currentLanguage;
            current = (current + offset) % (Enum.GetNames(typeof(Language)).Length);
            currentLanguage = (Language)current;
            
            OnChange?.Invoke(this);
        }
    }
}