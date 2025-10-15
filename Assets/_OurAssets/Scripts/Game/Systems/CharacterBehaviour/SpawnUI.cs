using TMPro;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace CursedOnion
{
    public class SpawnUI : MonoBehaviour
    {
        [SerializeField] private TurnSystem turnSystem;
        [SerializeField] private Button spawnButton;
        [SerializeField] private GameObject placingText; 

        private void Start()
        {
            turnSystem.OnSpawnPhaseEnded += HandleSpawnPhaseEnded;

            spawnButton.onClick.AddListener(OnSpawnButtonClicked);

            spawnButton.gameObject.SetActive(true);
            placingText.SetActive(false);
        }

        private void OnSpawnButtonClicked()
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

        private void OnDestroy()
        {
            turnSystem.OnSpawnPhaseEnded -= HandleSpawnPhaseEnded;
        }
    }
}
