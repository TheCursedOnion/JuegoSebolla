using CursedOnion.Game.Systems.Grid;
using UnityEngine;
using UnityEngine.Splines;

namespace CursedOnion
{
    public class MoveCommand : CharacterCommand
    {
        private Vector3 previousPosition;
        private Tile3d previousTile;
        private Tile3d newTile;
        private Grid3d grid;
        private Vector3 targetPosition;

        public MoveCommand(IEntity character, Vector3 newPosition, Grid3d levelgrid) : base(character)
        {
            this.targetPosition = newPosition;
            this.grid = levelgrid;
        }

        public override void Execute()
        {
            var characterObj = character as Character;
            if (characterObj != null)
            {
                previousPosition = characterObj.transform.position;

                if (grid.TryWorldToGridPosition(previousPosition, out Vector3 prevGridPos))
                {
                    Debug.Log("Previous Grid Position: " + prevGridPos);
                    previousTile = grid.GetTileAtGridPosition(prevGridPos);
                }

                if (grid.TryWorldToGridPosition(targetPosition, out Vector3 newGridPos))
                {
                    Debug.Log("New Grid Position: " + newGridPos);
                    newTile = grid.GetTileAtGridPosition(newGridPos);
                }
                Debug.Log(characterObj.characterName + characterObj.id + " Moving from " + previousPosition + " to " + targetPosition);

                previousTile?.SetContainedEntity(null);
                characterObj.Move(targetPosition);
                newTile?.SetContainedEntity(characterObj);
            }
        }


        public override void Undo()
        {
            Debug.Log("El comando de movimiento se ha DESHECHO");
            var characterObj = character as Character;
            if (characterObj != null)
            {
                newTile?.SetContainedEntity(null);
                characterObj.Move(previousPosition);
                previousTile?.SetContainedEntity(characterObj);
            }
        }

        public override void Redo()
        {
            Debug.Log("El comando de movimiento se ha VOLVIDO A HACER");
            Execute();
        }
    }
}
