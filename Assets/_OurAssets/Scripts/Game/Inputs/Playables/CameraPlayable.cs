using CursedOnion.Extensions;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CursedOnion.Game.Inputs
{
    public class CameraPlayable : MonoBehaviour, IPlayable
    {
        [Inject] public InputReader InputReader { get; set; }
        [field: SerializeField] public string UsedMap { get; set; }
        

        public void OnEnable()
        {
            if(InputReader == null) return;
            
            InputAction selectAction = InputReader.FindMapAction(UsedMap,"MovePointer");
            selectAction.started += MoveCamera;
        }

        public void OnDisable()
        {
            if(InputReader == null) return;
            
            InputAction selectAction = InputReader.FindMapAction(UsedMap,"MovePointer");
            selectAction.started -= MoveCamera;
        }

        void MoveCamera(InputAction.CallbackContext context)
        {
            Vector3 direction = context.ReadValue<Vector2>();
            direction = direction.SwizzleXZY();
            this.transform.Translate(direction.normalized);
        }
    }
}