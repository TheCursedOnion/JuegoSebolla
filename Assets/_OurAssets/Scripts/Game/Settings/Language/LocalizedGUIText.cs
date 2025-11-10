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
            languageSetting.OnChange += UpdateText;
            UpdateText(LanguageSetting.Language.Spanish);
        }
        void OnDisable()
        {
            languageSetting.OnChange -= UpdateText;
        }
        void UpdateText(LanguageSetting.Language _)
        {
            languageSetting ??= gameObject.scene.GetSceneContainer().Resolve<GameSettings>().LanguageSettings;
            textMesh.text = languageSetting.GetLocalizedString(!useMultipleKeys ? key : keys[useKeyIndex]);
        }
        
        public void SetUsedKey(int index)
        {
            useKeyIndex = index;
            UpdateText(LanguageSetting.Language.Spanish);
        }
    }
}
