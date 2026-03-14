
using System;

namespace Core.FSM
{
    /// <summary>
    /// Clase base abstracta para todos los estados de una máquina de estados.
    /// </summary>
    public abstract class State
    {
        protected StateMachine stateMachine;

        public State(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
        }

        #region Lifecycle Methods

        public virtual void OnEnter() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void OnExit() { }

        #endregion
    }

    /// <summary>
    /// Variante genérica de un estado que trabaja con un contexto.
    /// Útil para estados de jugador, enemigos u otros sistemas que requieren datos compartidos.
    /// </summary>
    /// <typeparam name="TContext"> Tipo de contexto que implementa </typeparam>
    public abstract class State<TContext> : State where TContext : IContext
    {
        protected TContext ctx;

        public State(StateMachine stateMachine, TContext ctx) : base(stateMachine)
        {
            this.ctx = ctx;
        }
    }

    /// <summary>
    /// Interfaz que define un contexto que puede ser utilizado por los estados
    /// Sirve como contrato para exponer datos y servicios necesarios dentro de los estados.
    /// </summary>
    public interface IContext { }
}