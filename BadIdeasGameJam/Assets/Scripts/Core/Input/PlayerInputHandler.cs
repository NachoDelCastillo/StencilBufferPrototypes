using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.CustomInput
{
    /// <summary> 
    /// Define todas las acciones de input del jugador que se quieren almacenar.
    /// </summary>
    public enum PlayerInputAction
    {
        Jump,
        Grab,
        InteractWithBox,
        Pause,
        SkipSong
    }

    public class PlayerInputHandler : InputHandler<PlayerInputHandler, PlayerInputAction>, InputMap.IPlayerMovementActions
    {
        #region Variables

        /// <summary>
        /// Input de movimiento del jugador. X y Y entre -1 y 1.
        /// </summary>
        public Vector2 MoveInput { get; private set; }

        #endregion

        #region MonoBehaviour

        protected override void Initialize()
        {
            base.Initialize();

            // Inicializar el Input System y asignar callbacks
            inputMap.PlayerMovement.SetCallbacks(this);
        }

        #endregion

        #region Enable

        protected override void EnableMap()
        {
            inputMap.PlayerMovement.Enable();
        }

        protected override void DisableMap()
        {
            inputMap.PlayerMovement.Disable();
        }

        #endregion

        #region Public Accessors

        /// <summary>
        /// Magnitud absoluta del input horizontal.
        /// </summary>
        public float MoveInputMagnitude => Mathf.Abs(MoveInput.magnitude);

        public int MoveInputXSign =>
            MoveInput.x > 0 ? 1 :
            MoveInput.x < 0 ? -1 : 0;

        #endregion

        #region Input Callbacks

        public void OnMove(InputAction.CallbackContext context) => MoveInput = context.ReadValue<Vector2>();

        public void OnJump(InputAction.CallbackContext context) => HandleInputActionState(PlayerInputAction.Jump, context);

        public void OnPause(InputAction.CallbackContext context) => HandleInputActionState(PlayerInputAction.Pause, context);

        public void OnGrab(InputAction.CallbackContext context) => HandleInputActionState(PlayerInputAction.Grab, context);

        public void OnInteractWithBox(InputAction.CallbackContext context) => HandleInputActionState(PlayerInputAction.InteractWithBox, context);

        #endregion
    }
}
