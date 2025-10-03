using UnityEngine;
using UnityEngine.Splines;

namespace CursedOnion
{
    public class MoveCommand : CharacterCommand
    {
        private Vector3 newPosition, previousPosition;

        public MoveCommand(IEntity character, Vector3 newPosition) : base(character) 
        {
            this.newPosition = newPosition;
        }

        public override void Execute()
        {
            var characterObj = character as Character;
            if (characterObj != null)
            {
                previousPosition = characterObj.transform.position;
                characterObj.Move(newPosition);
            }
        }


        public override void Undo()
        {
            Debug.Log("El comando de movimiento se ha DESHECHO");
            var characterObj = character as Character;
            if (characterObj != null)
            {
                characterObj.transform.position = previousPosition;
            }
        }

        public override void Redo()
        {
            Debug.Log("El comando de movimiento se ha VOLVIDO A HACER");
            Execute();
        }
    }
}
