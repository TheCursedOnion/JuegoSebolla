using UnityEngine;

namespace CursedOnion
{
    public class AttackCommand : CharacterCommand
    {
        IEntity target;
        int previousHP;

        public AttackCommand(IEntity character, IEntity target) : base(character) { }

        public override void Execute()
        {
            var targetObj = target as Character;
            if (targetObj != null)
            {
                previousHP = targetObj.HP;
            }
            character.Attack(target);
        }

        public override void Undo()
        {
            Debug.Log("El comando de ataque se ha DESHECHO");
            var targetObj = target as Character;
            if (targetObj != null)
            {
                targetObj.HP = previousHP;
            }
        }

        public override void Redo()
        {
            Debug.Log("El comando de ataque se ha VOLVIDO A HACER");
            character.Attack(target);
        }
    }


}
