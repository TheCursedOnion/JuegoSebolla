using Reflex.Extensions;
using TMPro;
using UnityEngine;

namespace CursedOnion.Game.Settings
{
    public class LanguagePanel : MonoBehaviour
    {
        LanguageSetting languageSetting;
        
        [SerializeField] private TextMeshProUGUI lenguageText;
        private void OnEnable()
        {
            languageSetting ??= gameObject.scene.GetSceneContainer().Resolve<GameSettings>().LanguageSettings;
            languageSetting.OnChange += UpdateText;
            UpdateText(languageSetting);
        }
        private void OnDisable()
        {
            if(languageSetting == null) return;
            
            languageSetting.OnChange -= UpdateText;
        }

        public void NextLanguage()
        {
            languageSetting.MoveLanguage(1);
        }
        public void PreviousLanguage()
        {
            languageSetting.MoveLanguage(-1);
        }
        void UpdateText(LanguageSetting setting)
        {
            lenguageText.text = setting.CurrentLanguage.ToString();
        }
    }
}
