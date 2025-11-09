using CursedOnion.Game.Localization;
using Reflex.Extensions;
using TMPro;
using UnityEngine;

namespace CursedOnion.Game.Settings
{
    public class LanguagePanel : MonoBehaviour
    {
        LanguageSetting languageSetting;
        
        [SerializeField] LocalizedGUIText localizedGUIText;
        private void Awake()
        {
            localizedGUIText ??= GetComponent<LocalizedGUIText>();
        }
        private void OnEnable()
        {
            languageSetting ??= gameObject.scene.GetSceneContainer().Resolve<GameSettings>().LanguageSettings;
            languageSetting.OnChange += UpdateText;
            UpdateText(languageSetting.GetCurrentLanguage());
        }
        private void OnDisable()
        {
            if(languageSetting == null) return;
            
            languageSetting.OnChange -= UpdateText;
        }

        public void NextLanguage()
        {
            languageSetting.MoveUsedLanguage(1);
        }
        public void PreviousLanguage()
        {
            languageSetting.MoveUsedLanguage(-1);
        }
        void UpdateText(LanguageSetting.Language language)
        {
            localizedGUIText.SetUsedKey((int)language);
        }
    }
}
