using System;
using CursedOnion.Game.Localization;
using CursedOnion.Game.Miscellaneous;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace CursedOnion.Game.Modes.Level.BattleEditor.UI
{
    public readonly struct StatLineParameters
    {
        public enum Mode { Slider, Value, Extra }

        public Mode DisplayMode { get; }
        public Vector2Int Range { get; }
        public Vector2Int SecondaryRange { get; }
        public string Text { get; }
        public bool ImprovedStat { get; }

        private StatLineParameters(Mode mode, bool improved, string text, Vector2Int range, Vector2Int secondaryRange)
        {
            DisplayMode = mode;
            Text = text;
            Range = range;
            SecondaryRange = secondaryRange;
            ImprovedStat = improved;
        }
        
        public static StatLineParameters Slider(Vector2Int range, Vector2Int secondaryRange, bool improved = false)
            => new(Mode.Slider, improved, null, range, secondaryRange);

        public static StatLineParameters Value(string text, bool improved = false)
            => new(Mode.Value, improved, text, default, default);

        public static StatLineParameters Extra(string text, bool improved = false)
            => new(Mode.Extra,improved, text, default, default);
    }
    
    [RequireComponent(typeof(LocalizedGUIText))]
    public class StatLine : MonoBehaviour
    {
        [SerializeField] private Image improvedIcon;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private TextMeshProUGUI extraText;
        
        [SerializeField] private bool usesSlider;
        [SerializeField, ShowIf("usesSlider")] private Slider slider;
        [SerializeField, ShowIf("usesSlider")] private Slider secondarySlider;

        public void Awake()
        {
            slider.gameObject.SetActive(usesSlider);
            valueText.gameObject.SetActive(!usesSlider);
            extraText.text = "";
            improvedIcon.gameObject.SetActive(false);
        }

        public void SetValue(StatLineParameters parameters)
        {
            switch (parameters.DisplayMode)
            {
                case StatLineParameters.Mode.Slider:
                    slider.gameObject.SetActive(true);
                    valueText.gameObject.SetActive(false);
                    
                    improvedIcon.gameObject.SetActive(parameters.ImprovedStat);
                    slider.value = (float)parameters.Range.x / parameters.Range.y;
                    
                    float secondaryValue = (float)parameters.SecondaryRange.x / parameters.SecondaryRange.y;
                    secondarySlider.value = Mathf.Max(secondaryValue, 0.08f); //Tamaño mínimo para que se vea bien
                    secondarySlider.gameObject.SetActive(parameters.ImprovedStat);
                    
                    break;

                case StatLineParameters.Mode.Value:
                    slider.gameObject.SetActive(false);
                    valueText.gameObject.SetActive(true);
                    
                    //improvedText.text = parameters.ImprovedStat ? "*" : "";
                    
                    valueText.text = parameters.Text;
                    improvedIcon.gameObject.SetActive(parameters.ImprovedStat);
                    extraText.text = "";
                    break;

                case StatLineParameters.Mode.Extra:
                    extraText.text = parameters.Text;
                    break;
            }
        }
    }
}
