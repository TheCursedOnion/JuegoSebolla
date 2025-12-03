using System;
using CursedOnion.Game.Settings;
using NaughtyAttributes;
using Reflex.Extensions;
using TMPro;
using UnityEngine;

namespace CursedOnion.Game.Localization
{
    public class LocalizedGUIText : MonoBehaviour
    {
        [SerializeField] private bool useMultipleKeys = false;
        
        [HideIf("useMultipleKeys"), SerializeField] string key;
        [ShowIf("useMultipleKeys"), SerializeField] string[] keys;
        [ShowIf("useMultipleKeys"), SerializeField] int useKeyIndex;
        
        [SerializeField] TextMeshProUGUI textMesh;
        
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
        
        public void SetKey(string key)
        {
            this.key = key;
            UpdateText();
        }
        public void SetNullKey()
        {
            this.key = "";
            UpdateText();
        }
        public void SetUsedKeyIndex(int index)
        {
            useKeyIndex = index;
            UpdateText();
        }
        void OnLanguageChange(LanguageSetting.Language _)
        {
            UpdateText();
        }

        void UpdateText()
        {
            if (string.IsNullOrEmpty(key) && !useMultipleKeys)
            {
                textMesh.text = "";
                return;
            }
            languageSetting ??= gameObject.scene.GetSceneContainer().Resolve<GameSettings>().LanguageSettings;
            textMesh.text = languageSetting.GetLocalizedString(!useMultipleKeys ? key : keys[useKeyIndex]);
        }
    }
}
