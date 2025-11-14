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
            languageSetting.OnChange += OnLenguageChange;
            UpdateText();
        }
        void OnDisable()
        {
            languageSetting.OnChange -= OnLenguageChange;
        }
        
        public void SetKey(string key)
        {
            this.key = key;
            UpdateText();
        }
        public void SetUsedKeyIndex(int index)
        {
            useKeyIndex = index;
            UpdateText();
        }
        void OnLenguageChange(LanguageSetting.Language _)
        {
            UpdateText();
        }

        void UpdateText()
        {
            languageSetting ??= gameObject.scene.GetSceneContainer().Resolve<GameSettings>().LanguageSettings;
            textMesh.text = languageSetting.GetLocalizedString(!useMultipleKeys ? key : keys[useKeyIndex]);
        }
    }
}
