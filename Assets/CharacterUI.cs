using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace CursedOnion.Game.Entity.UI
{
    public class CharacterUI : MonoBehaviour
    {
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI idText;

        public TextMeshProUGUI tutoText;
        public TextMeshProUGUI StatsText;

        public Button attackButton;
        public Button moveButton;

        private Character character;

        int currentHP;
        int maxHP;

        void Awake()
        {
            if (tutoText != null) tutoText.gameObject.SetActive(false);
            if (StatsText != null) StatsText.gameObject.SetActive(false);
            if (attackButton != null) attackButton.gameObject.SetActive(false);
            if (moveButton != null) moveButton.gameObject.SetActive(false);
        }
        public void UpdateStatsDisplay()
        {
            if (character == null || StatsText == null) return;

            currentHP = character.Stats.CurrentHealthStat;;
            //maxHP = Mathf.Max(1, maxHP); // seguridad
            //StatsText.text = ($"{character.characterName} -> {character.speedStat}\nID -> {character.id} \nHP -> {currentHP}/{maxHP}\nattack -> {character.attackStat}\ndefense -> {character.defenseStat}\nmovement -> {character.movementStat}\nprice -> {character.priceStat}");
        }

        public void SetStatsTrue()
        {
            UpdateStatsDisplay();
            if (StatsText != null) StatsText.gameObject.SetActive(true);
        }

        public void SetStatsFalse()
        {
            if (StatsText != null) StatsText.gameObject.SetActive(false);
        }

        public void SetButtonsTrue()
        {
            if (tutoText != null) tutoText.gameObject.SetActive(false);
            if (attackButton != null)
            {
                attackButton.gameObject.SetActive(true);
            }
            if (moveButton != null)
            {
                moveButton.gameObject.SetActive(true);
            }
            //RefreshButtonsState(character != null && character.hasMoved, character != null && character.hasAttacked, character != null && character.canMove, character != null && character.canAttack);
        }

        public void SetButtonsFalse()
        {
            if (attackButton != null) attackButton.gameObject.SetActive(false);
            if (moveButton != null) moveButton.gameObject.SetActive(false);
            if (tutoText != null) tutoText.gameObject.SetActive(false);
        }

        public void RefreshButtonsState(bool hasMoved, bool hasAttacked, bool canMove, bool canAttack)
        {
            if (attackButton != null)
            {
                attackButton.interactable = !hasAttacked;
            }

            if (moveButton != null)
            {
                moveButton.interactable = !hasMoved;
            }

            if (tutoText != null)
            {
                tutoText.gameObject.SetActive(canAttack || canMove);
            }
        }


        public void SetTextTutoAttack()
        {
            SetButtonsFalse();
            if (tutoText != null) tutoText.gameObject.SetActive(true);
            //if (character != null) character.canAttack = true;
            if (tutoText != null) tutoText.text = "Click at valid tile to Attack an Enemy!";
        }
        public void SetTextTutoMove()
        {
            SetButtonsFalse();
            if (tutoText != null) tutoText.gameObject.SetActive(true);
            //if (character != null) character.canMove = true;
            if (tutoText != null) tutoText.text = "Click at valid tile to Move!";
        }

        public void ShowForTurn()
        {
            gameObject.SetActive(true);
            UpdateStatsDisplay();
            SetButtonsTrue();
            SetStatsTrue();
            if (tutoText != null) tutoText.gameObject.SetActive(false);
        }

        public void ShowForSelection(bool isCurrentTurn)
        {
            gameObject.SetActive(true);
            if (isCurrentTurn)
            {
                ShowForTurn();
            }
            else
            {
                SetButtonsFalse();
                SetStatsTrue();
                if (tutoText != null) tutoText.gameObject.SetActive(false);
            }
        }

        public void HideUI()
        {
            gameObject.SetActive(false);
        }

    }
}
