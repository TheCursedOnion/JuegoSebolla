using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace CursedOnion.Game.Inputs
{
    [CreateAssetMenu(fileName = "BattleInputReader", menuName = "Game/Inputs/BattleInputs")]
    public class BattleInputReader : ScriptableObject, PlayerInputActions.IBattleActions, IInputReader
    {
        public PlayerInputActions InputActions { get; set; }
        public PlayerInputActions.BattleActions BattleActions;
        
        public event UnityAction<Vector2> MovePointer = delegate { };
            public Vector2 Direction => InputActions.Battle.MovePointer.ReadValue<Vector2>();
            
        public event UnityAction Select = delegate { };
            //public bool IsSelectPressed => InputActions.Battle.Select.IsPressed();
            
        public event UnityAction Inspect = delegate { };

        #region Enable & Disable
        static void SetupInputReader(BattleInputReader inputReader)
        {
            PlayerInputActions inputActions = new PlayerInputActions();
                
            PlayerInputActions.BattleActions battleActions = inputActions.Battle;
            battleActions.SetCallbacks(inputReader);
            
            inputReader.InputActions = inputActions;
            inputReader.BattleActions = battleActions;
        }
        public void Enable()
        {
            if (InputActions == null)
            {
                SetupInputReader(this);
            }
            BattleActions.Enable();
        }
        public void Disable()
        {
            if (InputActions == null)
            {
                SetupInputReader(this);
            }
            BattleActions.Disable();
        }
        #endregion
        
        
        #region Action Callbacks
        public void OnMovePointer(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started: MovePointer.Invoke(context.ReadValue<Vector2>()); break;
            }
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started: Select.Invoke(); break;
            }
        }

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
