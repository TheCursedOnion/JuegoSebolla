using UnityEngine;

namespace CursedOnion
{
    public class MoveCommand : CharacterCommand
    {
        private Vector3 previousPosition;

        public MoveCommand(IEntity character) : base(character) { }

        public override void Execute()
        {
            var characterObj = character as Character;
            if (characterObj != null)
            {
                previousPosition = characterObj.transform.position;
                characterObj.Move();
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
