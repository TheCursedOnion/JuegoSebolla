using CursedOnion.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace CursedOnion.Game.Inputs
{
    [CreateAssetMenu(fileName = "TileSelectorInputReader", menuName = "Game/Inputs/TileSelectorInputReader")]
    public class TileSelectorInputReader : ScriptableObject, PlayerInputActions.ITileSelectorActions, IInputReader
    {
        public PlayerInputActions InputActions { get; set; }
        public PlayerInputActions.TileSelectorActions TileSelectorActions;
        
        #region Enable & Disable
        void SetupInputReader()
        {
            PlayerInputActions inputActions = new PlayerInputActions();
                
            PlayerInputActions.TileSelectorActions selectorActions = inputActions.TileSelector;
            selectorActions.SetCallbacks(this);
            
            this.InputActions = inputActions;
            this.TileSelectorActions = selectorActions;
        }
        public void Enable()
        {
            if (InputActions == null)
            {
                SetupInputReader();
            }
            TileSelectorActions.Enable();
        }
        public void Disable()
        {
            if (InputActions != null)
            {
                TileSelectorActions.Disable();
            }
        }
        #endregion
        
        
        #region Action Callbacks
        public event UnityAction<Vector2> MoveSelector = delegate { };
        public void OnMoveSelector(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started: MoveSelector.Invoke(context.ReadValue<Vector2>()); break;
            }
        }
        
        public event UnityAction Select = delegate { };
        //public bool IsSelectPressed => InputActions.TileSelector.Select.IsPressed();
        public void OnSelect(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started: Select.Invoke(); break;
            }
        }
        
        public event UnityAction Inspect = delegate { };
        public void OnInspect(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started: Inspect.Invoke(); break;
            }
        }

        #endregion
    }
}
