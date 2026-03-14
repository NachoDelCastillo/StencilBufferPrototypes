using Core.FSM;
using System;
using UnityEngine;

namespace Game.Entities.Base
{
    /// <summary>
    /// Contexto para una entidad, que contiene referencias y datos compartidos
    /// que los estados de la máquina de estados pueden usar.
    /// </summary>
    public class EntityContext : IContext
    {
        #region Variables

        /// <summary>
        /// Máquina de estados asociada a esta entidad.
        /// Permite a los estados cambiar el flujo de comportamiento.
        /// </summary>
        public StateMachine StateMachine { get; }

        /// <summary>
        /// Velocidad de movimiento actual de la entidad.
        /// </summary>
        private Vector3 _movementVelocity;
        public Vector3 MovementVelocity
        {
            get
            {
                return _movementVelocity;
            }
            set
            {
                _movementVelocity = value;
                OnMovementVelocityChanged.Invoke(_movementVelocity);
            }
        }

        public void SetMovementVelocityYAxis(float yAxisValue)
        {
            MovementVelocity = new Vector3(MovementVelocity.x, yAxisValue, MovementVelocity.z);
        }

        public event Action<Vector3> OnMovementVelocityChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Crea un nuevo contexto para la entidad.
        /// </summary>
        /// <param name="stateMachine">La máquina de estados asociada a esta entidad.</param>
        public EntityContext(StateMachine stateMachine)
        {
            StateMachine = stateMachine ?? throw new System.ArgumentNullException(nameof(stateMachine));
        }

        #endregion
    }
}