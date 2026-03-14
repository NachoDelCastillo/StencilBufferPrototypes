using Core.Utils;
using System;
using UnityEngine;

namespace Core.Particles
{
    /// <summary>
    /// Manager genérico de partículas que permite mapear cualquier enum de tipos de partículas
    /// a ParticleSystems y reproducirlos mediante código.
    /// </summary>
    /// <typeparam name="TParticleId">Enum que define los tipos de partículas.</typeparam>
    public class ParticlesManager<TParticleId> : MonoBehaviour
        where TParticleId : Enum
    {
        #region Parameters

        [Header("Particle Systems")]
        [Tooltip("Diccionario que asocia cada tipo de partícula (enum) con su ParticleSystem.")]
        [SerializeField]
        private SerializedDictionary<TParticleId, ParticleSystem> particleSystems;

        #endregion

        #region Protected API

        /// <summary>
        /// Reproduce la partícula correspondiente al tipo indicado.
        /// </summary>
        /// <param name="particleId">Tipo de partícula a reproducir.</param>
        protected void PlayParticle(TParticleId particleId)
        {
            if (particleSystems == null)
            {
                Debug.LogWarning($"[{nameof(ParticlesManager<TParticleId>)}] Particle dictionary not assigned.", this);
                return;
            }

            particleSystems[particleId].Play();
        }

        #endregion
    }
}