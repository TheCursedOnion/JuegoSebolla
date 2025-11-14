using CursedOnion.Game.Localization;
using TMPro;
using UnityEngine;


namespace CursedOnion.Game.Modes.Level.BattleEditor.UI
{
    [RequireComponent(typeof(LocalizedGUIText))]
    public class StatLine : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI valueText;

        public void SetRangedValue(Vector2Int range)
        {
            string text = $"{range.x} - {range.y}";
            valueText.text = text;
        }
        public void SetValue(int value)
        {
            valueText.text = value.ToString();
        }
    }
}
