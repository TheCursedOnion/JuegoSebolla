using CursedOnion.Game.Inputs;
using CursedOnion.Game;
using CursedOnion.Helpers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace CursedOnion
{
    [CreateAssetMenu(fileName = "CameraInputReader", menuName = "Game/Inputs/CameraInputReader")]
    public class CameraInputReader : ScriptableObject, PlayerInputActions.ICameraActions, IInputReader
    {
        public PlayerInputActions InputActions { get; set; }
        public PlayerInputActions.CameraActions CameraActions;

        #region Enable & Disable
            void SetupInputReader()
            {
                PlayerInputActions inputActions = new PlayerInputActions();
                    
                PlayerInputActions.CameraActions cameraActions = inputActions.Camera;
                cameraActions.SetCallbacks(this);
                
                this.InputActions = inputActions;
                this.CameraActions = cameraActions;
            }
            public void Enable()
            {
                if (InputActions == null)
                {
                    SetupInputReader();
                }
                CameraActions.Enable();
            }

            public void Disable()
            {
                if (InputActions != null)
                {
                    CameraActions.Disable();
                }
            }
        #endregion
        
        public event UnityAction<Vector2> StartMove = delegate { };
        public event UnityAction<Vector2> Move = delegate { };
        public void OnMove(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started: StartMove.Invoke(context.ReadValue<Vector2>()); break;
            }
            Move.Invoke(context.ReadValue<Vector2>().normalized);
        }
        
        public event UnityAction<DirectionFlag> RotateCamera = delegate { };
        public void OnRotateCamera(InputAction.CallbackContext context)
        {
            float value = context.ReadValue<float>();
            if(value < 0.1f && value > -0.1f) return;
            
            DirectionFlag direction = value < 0 ? DirectionFlag.Left : DirectionFlag.Right;
            
            switch (context.phase)
            {
                case InputActionPhase.Started: RotateCamera.Invoke(direction); break;
            }
        }
    }
}
