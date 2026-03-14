using Core.FSM;
using System;
using System.Collections.Generic;

namespace Game.Entities.Base.Animation
{
    /// <summary>
    /// Gestiona los mapeos entre estados de la <see cref="Core.FSM.StateMachine"/> y animaciones.
    /// Permite:
    /// - Mapear un estado a una animación
    /// - Mapear una transición de estados a una animación
    /// - Definir animaciones que se reproducen automáticamente al finalizar otra animación
    /// </summary>
    /// <typeparam name="TAnimId">
    /// El enum que utiliza el AnimationSynchronizer para identificar el enum de las animaciones
    /// </typeparam>
    public class StateAnimationMappings<TAnimId>
        where TAnimId : Enum
    {
        #region Variables

        // Mapeo directo: State Type -> Anim Id
        private readonly Dictionary<Type, TAnimId> stateToAnimation = new Dictionary<Type, TAnimId>();

        // Mapeo de transiciones: (prevState, nextState) -> Anim Id
        private readonly Dictionary<(Type, Type), TAnimId> transitionToAnimation = new Dictionary<(Type, Type), TAnimId>();

        // Mapeo de animaciones que se deben reproducir al terminar otra animación
        private readonly Dictionary<TAnimId, TAnimId> onAnimationFinishMapping = new Dictionary<TAnimId, TAnimId>();

        #endregion

        #region Add Transitions

        /// <summary>
        /// Añade un mapeo directo de un estado a un identificador de animación.
        /// </summary>
        /// <typeparam name="TState">Tipo de estado que hereda de <see cref="State"/>.</typeparam>
        /// <param name="animationId">Identificador de animación asociado al estado.</param>
        public void AddStateAnimation<TState>(TAnimId animationId) where TState : State
        {
            stateToAnimation[typeof(TState)] = animationId;
        }

        /// <summary>
        /// Añade un mapeo de transición entre dos estados a un identificador de animación.
        /// </summary>
        /// <typeparam name="TFrom">Estado de origen.</typeparam>
        /// <typeparam name="TTo">Estado de destino.</typeparam>
        /// <param name="animationId">Animación a reproducir durante la transición.</param>
        public void AddTransitionAnimation<TFrom, TTo>(TAnimId animationId)
            where TFrom : State
            where TTo : State
        {
            transitionToAnimation[(typeof(TFrom), typeof(TTo))] = animationId;
        }

        /// <summary>
        /// Define qué animación reproducir automáticamente al finalizar otra animación.
        /// </summary>
        /// <param name="finishedAnimationId">Animación que ha finalizado.</param>
        /// <param name="animationToPlayId">Animación a reproducir después.</param>
        public void AddOnFinishAnimation(TAnimId finishedAnimationId, TAnimId animationToPlayId)
        {
            onAnimationFinishMapping[finishedAnimationId] = animationToPlayId;
        }

        #endregion

        #region Get Transitions

        /// <summary>
        /// Intenta obtener la animación asociada a un estado en tiempo de ejecución.
        /// </summary>
        public bool TryGetAnimation(Type stateType, out TAnimId animationId)
        {
            if (!typeof(State).IsAssignableFrom(stateType))
                throw new ArgumentException($"{stateType} must inherit from State", nameof(stateType));

            return stateToAnimation.TryGetValue(stateType, out animationId);
        }

        /// <summary>
        /// Intenta obtener la animación asociada a una transición de estados en tiempo de ejecución.
        /// </summary>
        public bool TryGetTransitionAnimation(Type fromState, Type toState, out TAnimId animationId)
        {
            if (fromState == null || toState == null)
            {
                animationId = default;
                return false;
            }

            if (!typeof(State).IsAssignableFrom(fromState) || !typeof(State).IsAssignableFrom(toState))
                throw new ArgumentException($"Both {fromState} and {toState} must inherit from State");

            return transitionToAnimation.TryGetValue((fromState, toState), out animationId);
        }

        /// <summary>
        /// Intenta obtener la animación que se debe reproducir cuando otra termina.
        /// </summary>
        public bool TryGetOnFinishAnimation(TAnimId finishedAnimationId, out TAnimId animationToPlayId)
        {
            return onAnimationFinishMapping.TryGetValue(finishedAnimationId, out animationToPlayId);
        }

        #endregion
    }
}