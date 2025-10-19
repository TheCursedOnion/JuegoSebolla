using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion
{
    public class SpawnUI : MonoBehaviour
    {
        [Inject] private LevelManager levelManager;
        TurnSystem turnSystem;
        
        [SerializeField] private Button spawnButton;
        [SerializeField] private GameObject placingText;

        void Awake()
        {
            turnSystem = levelManager.GetTurnSystem();
            Initialize();
        }

        void Initialize()
        {
            spawnButton.gameObject.SetActive(true);
            placingText.SetActive(false);
        }
        
        private void OnEnable()
        {
            turnSystem.OnSpawnPhaseEnded += HandleSpawnPhaseEnded;
        }
        public void OnSpawnButtonClicked()
        {
            turnSystem.canSpawnUnit = true;
            spawnButton.gameObject.SetActive(false);
            placingText.SetActive(true);
        }
        private void HandleSpawnPhaseEnded()
        {
            spawnButton.gameObject.SetActive(false);
            placingText.SetActive(false);
        }

        private void OnDisable()
        {
            turnSystem.OnSpawnPhaseEnded -= HandleSpawnPhaseEnded;
        }
    }
}
