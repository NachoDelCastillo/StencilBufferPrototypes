using Core.Singleton;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.CustomInput
{
    /// <summary>
    /// Componente encargado de gestionar el input.
    /// La intencion de utilidad de este componente es tener un componente que herede por cada capa de input diferente (PlayerInputComponent/UIInputComponent)
    /// </summary>
    public abstract class InputHandler<InputComponentClass, CustomInputAction> : Singleton<InputComponentClass>, IInputHandler
            where InputComponentClass : InputHandler<InputComponentClass, CustomInputAction>
    {
        #region Properties

        /// <summary>
        /// Si está activado, este InputHandler se registrará como el principal al iniciar.
        /// ADVERTENCIA: 
        /// - Si varios InputHandlers tienen este valor activo, 
        ///   el ÚLTIMO que despierte en escena será el que se convierta en el handler principal.
        /// - Unity mostrará un warning si detecta más de uno marcado.
        /// </summary>
        [SerializeField] private bool initEnable = true;

        #endregion

        #region Variables

        protected InputMap inputMap;

        /// <summary> 
        /// Diccionario que almacena el estado de cada acción del jugador.
        /// Esto permite añadir nuevas acciones sin modificar mucho código.
        /// </summary>
        private Dictionary<CustomInputAction, InputActionState> actionStates = new();

        /// <summary>
        /// Array cacheado de todas las acciones posibles
        /// </summary>
        private CustomInputAction[] allActions;

        #endregion

        #region Initialize

        protected override void Awake()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            // Cachear los valores del enum
            allActions = (CustomInputAction[])System.Enum.GetValues(typeof(CustomInputAction));

            // Inicializar el diccionario de acciones de input
            foreach (CustomInputAction action in System.Enum.GetValues(typeof(CustomInputAction)))
                actionStates[action] = new InputActionState();

            // Inicializar el Input System y asignar callbacks
            inputMap = new();

            if (initEnable)
            {
                // Detectar si YA existe un handler asignado antes de este
                if (InputRouter.Current != null)
                {
                    Debug.LogError(
                        $"[{GetType().Name}] Multiple InputHandlers have 'initEnable' set to TRUE. " +
                        $"Only ONE should be active at startup. " +
                        $"The last one initialized will override the previous one. " +
                        $"Current handler: {name}");
                    return;
                }

                // Este será el handler activo al iniciar la escena
                InputRouter.SetMainInputHandler(this);
            }
            else
            {
                // Aseguramos que arranca desactivado
                SetEnabled(false);
            }
        }

        #endregion

        #region MonoBehaviour

        private void LateUpdate()
        {
            // Limpiar los estados de las acciones al final del frame
            foreach (CustomInputAction action in allActions)
            {
                InputActionState state = actionStates[action];
                state.Clean();
                actionStates[action] = state;
            }
        }

        #endregion

        #region Enable

        // Activa/Desactiva este inputhandler
        // Integración con InputRouter
        public virtual void SetEnabled(bool enabled)
        {
            if (enabled)
                EnableMap();
            else
                DisableMap();
        }

        protected abstract void EnableMap();
        protected abstract void DisableMap();

        // Ejemplo de como overridear estos metodos
        //#region Enable
        //protected override void EnableMap()
        //{
        //    inputMap.UIControls.Enable();
        //}

        //protected override void DisableMap()
        //{
        //    inputMap.UIControls.Disable();
        //}
        //#endregion

        #endregion

        #region Public Accessors

        public bool IsPressed(CustomInputAction action) => actionStates[action].Pressed;
        public bool IsHeld(CustomInputAction action) => actionStates[action].Held;
        public bool IsReleased(CustomInputAction action) => actionStates[action].Released;

        #endregion

        #region Suscribe

        public void SubscribePressed(CustomInputAction action, Action callback)
        {
            actionStates[action].OnPressed += callback;
        }

        public void UnsubscribePressed(CustomInputAction action, Action callback)
        {
            actionStates[action].OnPressed -= callback;
        }

        public void SubscribeReleased(CustomInputAction action, Action callback)
        {
            actionStates[action].OnReleased += callback;
        }

        public void UnsubscribeReleased(CustomInputAction action, Action callback)
        {
            actionStates[action].OnReleased -= callback;
        }

        #endregion

        #region Input Callbacks

        /// <summary>
        /// Actualiza el estado de una acción de jugador según el contexto del Input System.
        /// </summary>
        /// <param name="inputAction">Acción a actualizar</param>
        /// <param name="context">Contexto proporcionado por Unity Input System</param>
        protected void HandleInputActionState(CustomInputAction inputAction, InputAction.CallbackContext context)
        {
            InputActionState state = actionStates[inputAction];

            if (context.performed)
            {
                state.Press();
            }
            else if (context.canceled)
            {
                state.Release();
            }

            actionStates[inputAction] = state;
        }

        #endregion

        #region Clear

        protected virtual void OnDestroy()
        {
            // Si este era el inputhandler actual, liberar al InputRouter
            // Comparacion explicita por referencia
            if ((object)InputRouter.Current == this)
            {
                InputRouter.ClearCurrent();
            }
        }

        #endregion
    }

    public static class InputRouter
    {
        private static IInputHandler current;
        public static IInputHandler Current => current;

        public static void SetMainInputHandler(IInputHandler handler)
        {
            if (handler == current)
                return;

            // Desactivar anterior
            current?.SetEnabled(false);

            // Activar nuevo
            current = handler;
            current?.SetEnabled(true);
        }

        public static void DisableInput()
        {
            current?.SetEnabled(false);
            current = null;
        }

        public static void ClearCurrent()
        {
            current = null;
        }
    }

    public interface IInputHandler
    {
        public void SetEnabled(bool enabled);
    }
}
