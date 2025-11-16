using System;
using System.Collections.Generic;
using CursedOnion.Tools;
using UnityEngine;

namespace CursedOnion.Game.Settings
{
    [Serializable]
    public class LanguageSetting : ISetting<LanguageSetting.Language>
    {
        public enum Language
        {
            Spanish = 0,
            English = 1
        }
        
        [SerializeField] Language currentLanguage;
            public Language GetCurrentLanguage() => currentLanguage;
            
        [SerializeField] string csvResourcePath;
        
        Dictionary<string, string[]> localizedData;
        public Action<LanguageSetting.Language> OnChange { get; set; }
        
        public void Initialize()
        {
            localizedData = CSVReader.LoadCsvResourceToDictionary(csvResourcePath, true);
        }
        
        public void SetUsedLanguage(Language language)
        {
            currentLanguage = language;
            OnChange?.Invoke(currentLanguage);
        }
        public void MoveUsedLanguage(int offset)
        {
            int current = (int)currentLanguage;
            int length = Enum.GetNames(typeof(Language)).Length;
            current = (((current + offset) % length) + length) % length;
            currentLanguage = (Language)current;
            
            OnChange?.Invoke(currentLanguage);
        }
        public string GetLocalizedString(string key)
        {
            if(localizedData == null) return "NOT INITIALIZED";
            
            if (!localizedData.ContainsKey(key)) return "KEY NOT FOUND";
                
            return localizedData[key][(int)currentLanguage];
        }
        public string[] GetLocalizedStrings(string key)
        {
            if (localizedData == null) return new[] { "NOT INITIALIZED" };

            if (!localizedData.ContainsKey(key)) return new[] { "KEY NOT FOUND" };
            
            return localizedData[key];
        }
    }
}