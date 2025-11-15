using CursedOnion.Game.Commands;
using CursedOnion.Game.Systems.Level;
using UnityEngine;

namespace CursedOnion.Game.Objects
{
    [System.Serializable]
    public class TileSelectorBehaviour : ObjectBehaviour
    {
        [SerializeField] protected TileSelector TileSelector;
        protected EntityCommandHandler CommandHandler;

        public void Initialize(TileSelector tileSelector, EntityCommandHandler commandHandler)
        {
            TileSelector = tileSelector;
            CommandHandler = commandHandler;
        }
        public virtual void HardSelect(SelectionData data) { }
        public virtual void SoftSelect(SelectionData data) { }
    }
}