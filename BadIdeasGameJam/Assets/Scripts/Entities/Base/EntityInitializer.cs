using UnityEngine;

namespace Game.Entities.Base
{
    /// <summary>
    /// Inicializador base genérico para cualquier entidad con <see cref="EntityController"/>.
    /// Su responsabilidad es cachear componentes comunes de la entidad como
    /// <see cref="EntityController{TContext}"/> y <see cref="EntityMovementComponent"/>.
    /// </summary>
    /// <typeparam name="TController">Tipo de controlador de la entidad.</typeparam>
    /// <typeparam name="TContext">Tipo de contexto asociado a la entidad.</typeparam>
    
    [RequireComponent(typeof(EntityMovementComponent))]
    public abstract class EntityInitializer<TController, TContext> : MonoBehaviour
        where TController : EntityController<TContext>
        where TContext : EntityContext
    {
        #region Variables

        /// <summary>
        /// Controlador de la entidad.
        /// </summary>
        protected TController entityController;

        /// <summary>
        /// Componente encargado del movimiento de la entidad.
        /// </summary>
        protected EntityMovementComponent moveComponent;

        #endregion

        #region MonoBehaviour

        protected virtual void Initialize()
        {
            entityController = GetComponent<TController>();
            moveComponent = GetComponent<EntityMovementComponent>();

            if (entityController == null)
                Debug.LogError($"[{nameof(EntityInitializer<TController, TContext>)}] No se encontró {typeof(TController).Name} en {gameObject.name}");

            if (moveComponent == null)
                Debug.LogError($"[{nameof(EntityInitializer<TController, TContext>)}] No se encontró {nameof(EntityMovementComponent)} en {gameObject.name}");
        }

        #endregion

        #region Getters

        public EntityMovementComponent GetEntityMovementComponent() => moveComponent;

        #endregion
    }
}