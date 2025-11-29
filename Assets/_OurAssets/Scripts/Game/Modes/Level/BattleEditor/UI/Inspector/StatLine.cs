using System;
using CursedOnion.Game.Localization;
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
        public string Text { get; }
        public bool ImprovedStat { get; }

        private StatLineParameters(Mode mode, bool improved, string text, Vector2Int range)
        {
            DisplayMode = mode;
            Text = text;
            Range = range;
            ImprovedStat = improved;
        }
        
        public static StatLineParameters Slider(Vector2Int range, bool improved = false)
            => new(Mode.Slider, improved, null, range);

        public static StatLineParameters Value(string text, bool improved = false)
            => new(Mode.Value, improved, text, default);

        public static StatLineParameters Extra(string text, bool improved = false)
            => new(Mode.Extra,improved, text, default);
    }
    
    [RequireComponent(typeof(LocalizedGUIText))]
    public class StatLine : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private TextMeshProUGUI extraText;
        
        [SerializeField] private bool usesSlider;
        [SerializeField, ShowIf("usesSlider")] private Slider slider;

        public void Awake()
        {
            slider.gameObject.SetActive(usesSlider);
            valueText.gameObject.SetActive(!usesSlider);
            extraText.text = "";
        }

        public void SetValue(StatLineParameters parameters)
        {
            switch (parameters.DisplayMode)
            {
                case StatLineParameters.Mode.Slider:
                    slider.gameObject.SetActive(true);
                    valueText.gameObject.SetActive(false);
                    slider.value = (float)parameters.Range.x / parameters.Range.y;
                    break;

                case StatLineParameters.Mode.Value:
                    slider.gameObject.SetActive(false);
                    valueText.gameObject.SetActive(true);
                    valueText.text = parameters.Text;
                    extraText.text = "";
                    break;

                case StatLineParameters.Mode.Extra:
                    extraText.text = parameters.Text;
                    break;
            }
        }
    }
}
