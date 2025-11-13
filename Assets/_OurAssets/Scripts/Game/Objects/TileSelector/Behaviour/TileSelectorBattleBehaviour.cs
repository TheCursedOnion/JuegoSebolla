using CursedOnion.Game.Commands;
using CursedOnion.Game.Systems.Grid;
using UnityEngine;

namespace CursedOnion.Game.Objects
{
    [System.Serializable]
    public class TileSelectorBattleBehaviour : TileSelectorBehaviour
    {
        public override void HardSelect(SelectionData data)
        {
            if (CommandHandler.HasPreparedCommand())
            {
                LaunchCommand(data.GridPosition, data.Tile);
            }
            else
            {
                SoftSelect(data);
            }
        }

        public override void SoftSelect(SelectionData data)
        {
            TileSelector.TrySelectEntity(data.Tile.GetContainedEntity());
        }

        void LaunchCommand(Vector3 gridPosition, Tile3d tile)
        {
            CommandParameters.Builder builder = new();
            builder.SetPosition(gridPosition);
            builder.SetTargetEntity(tile.GetContainedEntity());
            
            CommandParameters commandParameters = builder.Build();
            CommandHandler.ExecuteCommand(commandParameters);
        }
    }
}
