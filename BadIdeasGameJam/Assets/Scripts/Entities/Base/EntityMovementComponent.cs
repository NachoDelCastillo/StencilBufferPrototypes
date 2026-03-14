using System;
using UnityEngine;

namespace Game.Entities.Base
{
    /// <summary>
    /// Componente responsable del movimiento de la entidad (jugador, enemigo, etc.).
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class EntityMovementComponent : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool printDebugLogs;

        #region References

        [Header("References")]
        [SerializeField] private Rigidbody rb;

        #endregion

        #region Ground Detection Parameters

        [Space(10)]
        [Header("Ground Detection")]
        [Space(5)]

        [Tooltip("Layer para las colisiones de suelo")]
        [SerializeField] private LayerMask groundLayer;

        [SerializeField] private float groundCheckerRadius = 0.2f;

        [SerializeField] private bool debugShowGroundCollision;

        #endregion

        #region Flags & Movement Variables

        private bool onGround;

        // Parametro que se modifica desde los controllers para modificar el angulo actual de esta entidad
        Vector3 movementVelocity;

        #endregion

        #region Events

        public event Action OnGroundEnter;
        public event Action OnGroundExit;

        public event Action<Vector3> OnMovementVelocityApplied;

        #endregion

        #region Movement Methods

        /// <summary>
        /// Establece la velocidad de movimiento de la entidad.
        /// </summary>
        /// <param name="moveVelocity">Velocidad deseada.</param>
        public void SetMovementVelocity(Vector3 movementVelocity) => this.movementVelocity = movementVelocity;

        /// <summary>
        /// Aplica la velocidad de movimiento al Rigidbody.
        /// </summary>
        public void ApplyMovementVelocity()
        {
            rb.linearVelocity = movementVelocity;
            OnMovementVelocityApplied?.Invoke(movementVelocity);
        }

        #endregion

        #region Getters

        public Vector3 MovementVelocity => movementVelocity;
        public bool OnGround => onGround;

        public LayerMask GroundLayer => groundLayer;

        #endregion

        #region Collision Checks

        /// <summary>
        /// Actualiza los flags de collision de la entidad
        /// </summary>
        public void UpdateCollisionFlags()
        {
            OnGroundCheck();
        }

        /// <summary>
        /// Comprueba si la entidad está tocando el suelo.
        /// Dispara eventos OnGroundEnter y OnGroundExit.
        /// </summary>
        private void OnGroundCheck()
        {
            // Punto central de la detección, justo debajo del collider de los pies
            Vector3 sphereOrigin = transform.position;

            // Detecta si la esfera toca algo del groundLayer
            Collider[] hits = Physics.OverlapSphere(sphereOrigin, groundCheckerRadius, groundLayer);

            bool isGrounded = hits.Length > 0;

            if (isGrounded)
                EnterGround();
            else
                ExitGround();
        }

        void EnterGround()
        {
            if (!onGround)
            {
                onGround = true;
                OnGroundEnter?.Invoke();
            }
        }

        void ExitGround()
        {
            if (onGround)
            {
                onGround = false;
                OnGroundExit?.Invoke();
            }
        }

        #endregion

        #region Debug Visualization

        // Dibuja la esfera de detección en el editor cuando el objeto está seleccionado
        private void OnDrawGizmosSelected()
        {
            if (debugShowGroundCollision)
            {
                Gizmos.color = onGround ? Color.green : Color.yellow;
                Gizmos.DrawWireSphere(transform.position, groundCheckerRadius);
            }
        }

        #endregion
    }
}