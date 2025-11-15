using System;
using CursedOnion.Game.Events;
using CursedOnion.Game.Systems.Level;
using Reflex.Attributes;
using Reflex.Extensions;
using TMPro;
using UltEvents;
using UnityEngine;

namespace CursedOnion.Game.Modes.General.UI
{
    public class GoldCounter : MonoBehaviour
    {
        [Inject] LevelManager levelManager;
        
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] UltEvent OnNotEnoughGold;

        private void Awake()
        {
            levelManager = gameObject.scene.GetSceneContainer().Resolve<LevelManager>();
        }

        private void OnEnable()
        {
            levelManager.LevelEvents.OnGoldUpdated += UpdateGoldText;
            levelManager.LevelEvents.OnNotEnoughGold += DoNotEnoughGold;
            UpdateGoldText(levelManager.LevelScoreVariables.RemainingGold);
        }

        private void OnDisable()
        {
            levelManager.LevelEvents.OnNotEnoughGold -= DoNotEnoughGold;
            levelManager.LevelEvents.OnGoldUpdated -= UpdateGoldText;
        }

        void UpdateGoldText(int gold)
        {
            goldText.text = gold.ToString();
        }
        void DoNotEnoughGold()
        {
            OnNotEnoughGold.Invoke();
        }
    }
}
