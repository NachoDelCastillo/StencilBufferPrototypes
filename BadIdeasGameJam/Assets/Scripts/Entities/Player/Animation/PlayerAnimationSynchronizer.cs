using DG.Tweening;
using Game.Entities.Base;
using Game.Entities.Base.Animation;
using Game.Entities.Player.States;
using System;
using UnityEngine;

namespace Game.Entities.Player.Animation
{
    /// <summary>
    /// Sincronizador de animaciones específico para el jugador.
    /// Hereda de EntityAnimationSynchronizer para gestionar animaciones basadas en estados
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class PlayerAnimationSynchronizer : EntityAnimationSynchronizer<PlayerAnimationSynchronizer.PlayerAnim>
    {
        #region Parameters

        private const string horizontalMovementBlendTreeId = "HorizontalMovementVelocity";

        #endregion

        #region Variables

        /// <summary>
        /// Enums de animaciones disponibles para el jugador.
        /// </summary>
        public enum PlayerAnim
        {
            HorizontalMovementBT, // Blend Tree
            AirUp,
            AirDown,
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Inicializa el sincronizador de animaciones con los eventos del PlayerController.
        /// </summary>
        /// <param name="playerController">Referencia al controlador del jugador.</param>
        public void Initialize(PlayerController playerController)
        {
            base.Initialize(playerController.GetStateMachine());

            playerController.PlayerEvents.OnHorizontalMovementVelocityChangedNormalized.AddListener(OnHorizontalMovementVelocityChangedNormalized);

            // Suscribirse a eventos del jugador
            playerController.PlayerEvents.OnJump.AddListener(() => PlayAnimation(PlayerAnim.AirUp, .25f));
            playerController.PlayerEvents.OnStartFallingDown.AddListener(() => PlayAnimation(PlayerAnim.AirDown, .7f));
        }

        #endregion

        #region Animation Mappings

        /// <summary>
        /// Define mapeos de animación específicos del jugador
        /// </summary>
        protected override void SetupMappings()
        {
            // Mapear estados directamente con animaciones
            animatorStateMappings.AddStateAnimation<IdlePlayerState>(PlayerAnim.HorizontalMovementBT);
            animatorStateMappings.AddStateAnimation<RunPlayerState>(PlayerAnim.HorizontalMovementBT);

            //// Transiciones específicas
            //animatorStateMappings.AddTransitionAnimation<AirPlayerState, IdlePlayerState>(PlayerAnim.Land);

            //// Animaciones encadenadas al final de otras
            //animatorStateMappings.AddOnFinishAnimation(PlayerAnim.Land, PlayerAnim.HorizontalMovementBT);
        }

        #endregion

        #region Events

        // Actualizar el parametro de velocidad horizontal
        private void OnHorizontalMovementVelocityChangedNormalized(float horizontalMovementVelocityNormalized)
        {
            SetFloat(horizontalMovementBlendTreeId, horizontalMovementVelocityNormalized);
        }

        #endregion
    }
}