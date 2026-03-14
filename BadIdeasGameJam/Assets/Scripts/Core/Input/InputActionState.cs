using System;
using UnityEngine;

namespace Core.CustomInput
{
    /// <summary>
    /// Representa el estado de una acción de jugador en un frame.
    /// </summary>
    public class InputActionState
    {
        private bool pressed;
        private bool held;
        private bool released;

        public bool Pressed => pressed;
        public bool Held => held;
        public bool Released => released;

        public event Action OnPressed;
        public event Action OnReleased;

        /// <summary>
        /// Actualiza los flags al cuando se presiona esta accion
        /// </summary>
        public void Press()
        {
            pressed = true;
            held = true;

            OnPressed?.Invoke();
        }

        /// <summary>
        /// Actualiza los flags al cuando se suelta esta accion
        /// </summary>
        public void Release()
        {
            released = true;
            held = false;

            OnReleased?.Invoke();
        }

        /// <summary>
        /// Limpia los flags Pressed y Released
        /// </summary>
        public void Clean()
        {
            pressed = false;
            released = false;
        }
    }
}