using Core.Particles;
using System;
using UnityEngine;

namespace Game.Entities.Player
{
    /// <summary>
    /// Gestiona los efectos de partículas asociados al jugador.
    /// Extiende de <see cref="ParticlesManager{TParticleEnum}"/> para reutilizar la lógica genérica 
    /// de gestión y encapsula únicamente la lógica específica del jugador.
    public class PlayerParticleManager : ParticlesManager<PlayerParticleManager.PlayerParticles>
    {
        #region Enum

        /// <summary>
        /// Tipos de partículas exclusivas del jugador.
        /// </summary>
        public enum PlayerParticles
        {
            step,
            jump,
            land
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Inicializa el sistema de partículas del jugador, enlazando los eventos del controlador con las particulas
        /// </summary>
        /// <param name="playerController">Instancia del controlador del jugador cuyas acciones dispararán partículas.</param>
        public void Initialize(PlayerController playerController)
        {
            playerController.PlayerEvents.OnLand.AddListener(() => PlayParticle(PlayerParticles.land));
            playerController.PlayerEvents.OnJump.AddListener(() => PlayParticle(PlayerParticles.jump));
        }

        #endregion

        public void OnFootstepTriggered(FootstepId footstepId)
        {
            PlayParticle(PlayerParticles.step);
        }
    }
}