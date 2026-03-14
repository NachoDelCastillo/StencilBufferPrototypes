using Core.FSM;
using UnityEngine;

namespace Game.Entities.Base
{
    /// <summary>
    /// Controlador base para entidades con FSM, movimiento y ciclo de vida.
    /// Define la estructura para actualizar la máquina de estados y manejar movimiento.
    /// </summary>
    /// <typeparam name="TContext">Tipo de contexto asociado a esta entidad.</typeparam>
    public abstract class EntityController<TContext> : MonoBehaviour
        where TContext : EntityContext
    {
        #region Variables

        [SerializeField] bool stateMachineEnabled = true;

        /// <summary>
        /// Componente encargado del movimiento de la entidad.
        /// </summary>
        protected EntityMovementComponent moveComponent;

        /// <summary>
        /// Máquina de estados de la entidad.
        /// </summary>
        protected StateMachine stateMachine;

        /// <summary>
        /// Contexto asociado a la entidad, compartido por los estados.
        /// </summary>
        protected TContext ctx;

        #endregion

        #region Public Methods

        public StateMachine GetStateMachine() => stateMachine;

        public virtual void Initialize(EntityMovementComponent moveComponent)
        {
            this.moveComponent = moveComponent;

            stateMachine = new StateMachine();
            ctx = CreateContext(stateMachine);

            if (stateMachineEnabled)
                AddStates(stateMachine);
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Cada subclase debe implementar la creación de su contexto.
        /// </summary>
        /// <param name="stateMachine">La máquina de estados asociada a la entidad.</param>
        protected abstract TContext CreateContext(StateMachine stateMachine);

        /// <summary>
        /// Añade los estados que se usaran para esta entidad
        /// </summary>
        /// <param name="stateMachine">La máquina de estados asociada a la entidad.</param>
        protected abstract void AddStates(StateMachine stateMachine);

        #endregion

        #region MonoBehaviour Lifecycle

        protected virtual void Update()
        {
            if (!stateMachineEnabled) return;

            stateMachine?.Update();
        }

        protected virtual void FixedUpdate()
        {
            if (!stateMachineEnabled) return;

            UpdateCollisionFlags();
            stateMachine?.FixedUpdate();

            moveComponent.SetMovementVelocity(ctx.MovementVelocity);
            moveComponent.ApplyMovementVelocity();
        }

        #endregion

        #region Collision & Context Updates

        /// <summary>
        /// Actualiza flags de colisión y permite que subclases actualicen el contexto.
        /// </summary>
        protected virtual void UpdateCollisionFlags()
        {
            moveComponent.UpdateCollisionFlags();
            // Subclases pueden actualizar ctx según los flags de colision
        }

        #endregion
    }
}