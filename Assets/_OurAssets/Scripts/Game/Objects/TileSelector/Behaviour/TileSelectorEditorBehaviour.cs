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
            if (CommandHandler.HasPreparedCommand())
            {
                CommandParameters.Builder builder = new();
                builder.SetPosition(TileSelector.transform.position);
                builder.SetTargetTile(data.Tile);
                LaunchCommand(builder.Build());
            }
        }
        public override void SoftSelect(SelectionData data)
        {
            if (!CommandHandler.HasPreparedCommand())
                TileSelector.InvokeEntitySelection(data.Tile.GetContainedEntity());
        }
        void LaunchCommand(CommandParameters parameters)
        {
            CommandHandler.ExecuteCommand(parameters);
        }
    }
}