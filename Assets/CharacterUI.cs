using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CursedOnion
{
    public class CharacterUI : MonoBehaviour
    {
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI idText;

        private Character character;

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
    }
}
