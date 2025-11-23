using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CursedOnion.Game.CloudSave;
using CursedOnion.Tools;
using Unity.Services.CloudSave.Models;
using UnityEngine;

namespace CursedOnion.Game.Settings
{
    [Serializable]
    public class LanguageSetting : ICloudStorable
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
        Language GetApplicationLanguage()
        {
            var languageName = Application.systemLanguage;
            switch (languageName)
            {
                case SystemLanguage.Spanish: return Language.Spanish;
                default: return Language.English;
            }
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
        
        #region Cloud Storing
        const string LANGUAGE = "language";
        public void SaveInto(Dictionary<string, object> serializableData)
        {
            serializableData[LANGUAGE] = (int)currentLanguage;
        }
        public void LoadFrom(Dictionary<string, Item> loadedData)
        {
            localizedData ??= CSVReader.LoadCsvResourceToDictionary(csvResourcePath, true);
            
            int defaultLanguage = (int)GetApplicationLanguage();
            int usedLanguage = CloudUtils.GetValueFromQuery<int>(loadedData, LANGUAGE, defaultLanguage);
            
            SetUsedLanguage((Language)usedLanguage);
        }
        #endregion
    }
}