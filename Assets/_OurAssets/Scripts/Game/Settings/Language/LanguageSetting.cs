using System;
using System.Collections.Generic;
using CursedOnion.Game.CloudSave;
using CursedOnion.Tools;
using UnityEngine;

namespace CursedOnion.Game.Settings
{
    [Serializable]
    public class LanguageSetting : ISetting<LanguageSetting.Language>, ICloudStorable
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
            SetApplicationLanguage();
        }
        void SetApplicationLanguage()
        {
            var languageName = Application.systemLanguage;
            switch (languageName)
            {
                case SystemLanguage.Spanish: SetUsedLanguage(Language.Spanish); break;
                default: SetUsedLanguage(Language.English); break;
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
        public CloudSaveClient SaveClient { get; set; }
        public async void Save()
        {
            try
            {
                await SaveClient.Save("language", (int)currentLanguage);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error al guardar: " + e);
            }
        }
        public async void Load()
        {
            try
            {
                var language = await SaveClient.Load<int>("language");
                SetUsedLanguage((Language)language);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error al cargar: " + e);
            }
        }
        #endregion
    }
}