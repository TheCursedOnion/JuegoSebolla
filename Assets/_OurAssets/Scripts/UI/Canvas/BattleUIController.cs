using CursedOnion.UI;
using UnityEngine;

namespace CursedOnion.UI
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