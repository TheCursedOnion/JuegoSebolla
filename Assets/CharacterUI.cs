using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CursedOnion
{
    public class CharacterUI : MonoBehaviour
    {
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI idText;

        public TextMeshProUGUI tutoText;

        public Button attackButton;
        public Button moveButton;

        private Character character;

        public void Start()
        {
            attackButton.gameObject.SetActive(true);
            moveButton.gameObject.SetActive(true);
        }

        public void SetCharacter(Character c)
        {
            character = c;

            if (nameText != null)
                nameText.text = character.characterName;

            if (idText != null)
            {
                idText.text = character.id.ToString();
            }
        }

        public void UpdateUI()
        {
            if (character != null)
            {
                tutoText.gameObject.SetActive(false);
                attackButton.gameObject.SetActive(true);
                moveButton.gameObject.SetActive(true);
            }
        }

        public void SetButtonsFalse()
        {
            attackButton.gameObject.SetActive(false);
            moveButton.gameObject.SetActive(false);
        }

        public void SetTextTutoAttack()
        {
            attackButton.gameObject.SetActive(false);
            moveButton.gameObject.SetActive(false);
            tutoText.gameObject.SetActive(true);
            character.canAttack = true;
            tutoText.text = "Click at valid tile to Attack an Enemy!";
        }
        public void SetTextTutoMove()
        {
            attackButton.gameObject.SetActive(false);
            moveButton.gameObject.SetActive(false);
            tutoText.gameObject.SetActive(true);
            character.canMove = true;
            tutoText.text = "Click at valid tile to Move!";
        }

    }
}
