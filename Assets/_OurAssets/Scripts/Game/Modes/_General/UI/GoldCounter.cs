using System;
using CursedOnion.Game.Events;
using CursedOnion.Game.Systems.Level;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace CursedOnion.Game.Modes.General.UI
{
    public class GoldCounter : MonoBehaviour
    {
        [Inject] LevelManager levelManager;
        [SerializeField] private TextMeshProUGUI goldText;

        private void Awake()
        {
            //UpdateGoldText(levelEvents.RemainingGold);
        }

        private void OnEnable()
        {
            levelManager.LevelEvents.OnGoldUpdated += UpdateGoldText;
            UpdateGoldText(levelManager.LevelScoreVariables.RemainingGold);
        }

        private void OnDisable()
        {
            levelManager.LevelEvents.OnGoldUpdated -= UpdateGoldText;
        }

        void UpdateGoldText(int gold)
        {
            goldText.text = $"Dinero: {gold.ToString()}";
        }
    }
}
