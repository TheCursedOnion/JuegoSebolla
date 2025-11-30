using CursedOnion.Game.Entity.Effects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.Game.General.UI.Canvases.Level
{
    public class EffectIcon : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI effectDuration;

        public void AssignEffect(StatusEffect effect)
        {
            icon.sprite = effect.GetData().Icon;
            
            int remainingTurns = effect.GetRemainingDuration();
            string durationText = remainingTurns > 0 ? remainingTurns.ToString(): string.Empty;
            effectDuration.text = durationText;
        }
    }
}