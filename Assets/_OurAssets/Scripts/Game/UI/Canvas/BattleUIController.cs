using UnityEngine;

namespace CursedOnion.Game.UI
{
    public class BattleUIController : MonoBehaviour, IUIController
    {
        [SerializeField] private GameObject BattleUIParent;
        
        public void SetEnabled(bool enabled)
        {
            BattleUIParent.SetActive(enabled);
        }
    }
}