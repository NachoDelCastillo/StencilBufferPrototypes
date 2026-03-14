using UnityEngine.Events;

namespace Game.Entities.Player
{
    /// <summary>
    /// Contiene los eventos relacionados con las acciones del jugador.
    /// </summary>
    public class PlayerEvents
    {
        /// <summary>
        /// Se invoca cuando el jugador realiza un salto.
        /// </summary>
        public UnityEvent OnJump = new UnityEvent();

        /// <summary>
        ///  Se invoca cuando el jugador empieza a descender en el aire.
        /// </summary>
        public UnityEvent OnStartFallingDown = new UnityEvent();

        /// <summary>
        /// Se invoca cuando el jugador toca el suelo después de estar en el aire.
        /// </summary>
        public UnityEvent OnLand = new UnityEvent();

        /// <summary>
        /// Se invoca cuando la velocidad de movimiento de la entidad cambia
        /// Viene en valor normalizado respecto a la velocidad maxima en ese momento
        /// </summary>
        public UnityEvent<float> OnHorizontalMovementVelocityChangedNormalized = new UnityEvent<float>();
    }
}