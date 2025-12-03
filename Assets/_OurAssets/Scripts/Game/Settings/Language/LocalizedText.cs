using CursedOnion.Game.Settings;
using NaughtyAttributes;
using Reflex.Extensions;
using TMPro;
using UnityEngine;

namespace CursedOnion.Game.Localization
{
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] TextMeshPro textMesh;

        private bool usesKey = false;
        string key;
        string text;
        LanguageSetting languageSetting;
        
        void OnEnable()
        {
            languageSetting ??= gameObject.scene.GetSceneContainer().Resolve<GameSettings>().LanguageSettings;
            languageSetting.OnChange += OnLanguageChange;
            UpdateText();
        }
        void OnDisable()
        {
            languageSetting.OnChange -= OnLanguageChange;
        }
        
        void OnLanguageChange(LanguageSetting.Language _)
        {
            UpdateText();
        }

        public void SetKey(string key)
        {
            this.key = key;
            usesKey = true;
            UpdateText();
        }
        public void SetText(string text)
        {
            this.text = text;
            usesKey = false;
            UpdateText();
        }
        void UpdateText()
        {
            if (!usesKey || string.IsNullOrEmpty(key))
            {
                if (string.IsNullOrEmpty(text)) return;
                
                textMesh.text = text;
            }
            else
            {
                languageSetting ??= gameObject.scene.GetSceneContainer().Resolve<GameSettings>().LanguageSettings;
                textMesh.text = languageSetting.GetLocalizedString(key);
            }
        }
    }
}