using System;
using CursedOnion.Extensions;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CursedOnion.Game.Inputs
{
    public class CameraPlayable : MonoBehaviour, IPlayable
    {
        [Inject] public InputReaderCollection InputReaderCollection { get; set; }
        Vector2 moveInput;

        public void OnEnable()
        {
            Enable();
        }

        public void Enable()
        {
            BattleInputReader reader = InputReaderCollection.GetReader<BattleInputReader>();
            if (InputReaderCollection == null)
            {
                transform.localEulerAngles = new Vector3(-30, 0, 0);
                return;
            }
                
            reader.MovePointer += MoveCamera;
            
            reader.Enable();
        }
        
        public void OnDisable()
        {
            //Disable();
        }
        public void Disable()
        {
            /*BattleInputReader reader = InputReaderCollection.GetReader<BattleInputReader>();
            reader.MovePointer -= MoveCamera;*/
        }

        void MoveCamera(Vector2 direction)
        {
            moveInput = direction;
            
            Vector3 direction3D = moveInput;
            direction3D = direction3D.SwizzleXZY();
            transform.Translate(direction3D.normalized);
        }
    }
}