using CursedOnion.Game.Commands;
using CursedOnion.Game.Systems.Grid;
using UnityEngine;

namespace CursedOnion.Game.Objects
{
    [System.Serializable]
    public class TileSelectorEditorBehaviour : TileSelectorBehaviour
    {
        public override void HardSelect(SelectionData data)
        {
            if (CommandHandler.HasPreparedCommand() && data.Tile.GetContainedEntity() == null)
            {
                LaunchCommand(TileSelector.transform.position);
            }
        }
        public override void SoftSelect(SelectionData data)
        {
            if (!CommandHandler.HasPreparedCommand())
                TileSelector.SelectEntity(data.Tile.GetContainedEntity());
        }
        void LaunchCommand(Vector3 position)
        {
            CommandParameters.Builder builder = new();
            builder.SetPosition(position);
            CommandHandler.TriggerCommand(builder.Build());
        }
    }
}