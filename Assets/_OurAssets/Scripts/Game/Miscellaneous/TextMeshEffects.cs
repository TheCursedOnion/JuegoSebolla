using TMPro;
using UnityEngine;

namespace CursedOnion.Game.Miscellaneous
{
    public class TextMeshEffects : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMesh;
        
        public void Underline(bool enable)
        {
            FontStyles flag = FontStyles.Underline;
            if (enable)
            {
                EnableFlag(flag);
            }
            else
            {
                DisableFlag(flag);
            }
        }
        public void Bold(bool enable)
        {
            FontStyles flag = FontStyles.Bold;
            if (enable)
            {
                EnableFlag(flag);
            }
            else
            {
                DisableFlag(flag);
            }
        }
        void EnableFlag(FontStyles flag)
        {
            textMesh.fontStyle |= flag;
        }
        void DisableFlag(FontStyles flag)
        {
            textMesh.fontStyle &= ~flag;
        }
    }
}
