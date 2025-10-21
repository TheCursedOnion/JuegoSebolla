using CursedOnion.Game.Entity;
using CursedOnion.Game.Systems.Grid;
using UnityEngine;
using UnityEngine.Splines;

namespace CursedOnion.Game.Commands
{
    public class MoveCommand : EntityCommand
    {
        private Vector3 previousPosition;
        private Tile3d previousTile;
        private Tile3d newTile;
        private Grid3d grid;
        private Vector3 targetPosition;

        private bool previousHasMoved;

        public MoveCommand(ICommandEntity entity, Vector3 newPosition, Grid3d levelgrid) : base(entity)
        {
            this.targetPosition = newPosition;
            this.grid = levelgrid;
        }

        public override void Execute()
        {
            if (Entity != null)
            {
                previousPosition = Entity.Transform.position;
                previousHasMoved = Entity.Flags.HasMoved;

                previousTile = grid.GetTileAtWorldPosition(previousPosition);
                newTile = grid.GetTileAtWorldPosition(targetPosition);
                
                Debug.Log(Entity.Name + " Moving from " + previousPosition + " to " + targetPosition);

                previousTile?.SetContainedEntity(null);
                Entity.Move(targetPosition);
                newTile?.SetContainedEntity(Entity);
            }
        }


        public override void Undo()
        {
            Debug.Log("El comando de movimiento se ha DESHECHO");
            var character = Entity as Character;
            if (character != null)
            {
                character.StopAllCoroutines();

                character.uiScript.SetButtonsFalse();

                newTile?.SetContainedEntity(null);
                previousTile?.SetContainedEntity(character);

                character.Move(previousPosition);

                character.Flags.HasMoved = previousHasMoved;
            }
        }

        public override void Redo()
        {
            Debug.Log("El comando de movimiento se ha VOLVIDO A HACER");
            Execute();
        }
    }
}
