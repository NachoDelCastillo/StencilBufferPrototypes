using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.FSM
{
    /// <summary>
    /// Máquina de estados finita que gestiona la lógica de transición y actualización de estados.
    /// </summary>
    public class StateMachine
    {
        #region Variables

        private State currentState;

        /// <summary>
        /// Diccionario que guarda instancias de todos los estados gestionados por esta máquina.
        /// </summary>
        private readonly Dictionary<Type, State> states = new Dictionary<Type, State>();

        /// <summary>
        /// Evento lanzado al cambiar de estado.
        /// Parámetros: estado previo, estado siguiente.
        /// </summary>
        public event Action<Type, Type> OnStateChanged;

        #endregion

        #region State Management

        /// <summary>
        /// Agrega un estado a la máquina.
        /// </summary>
        /// <param name="state">Instancia de estado a agregar</param>
        public void AddState(State state)
        {
            if (state == null)
            {
                Debug.LogError("Cannot add null state to StateMachine.");
                return;
            }

            var type = state.GetType();
            if (states.ContainsKey(type))
            {
                Debug.LogWarning($"StateMachine already contains a state of type {type}. Ignoring AddState.");
                return;
            }

            states.Add(type, state);
        }

        /// <summary>
        /// Cambia el estado actual de la maquina.
        /// </summary>
        /// <typeparam name="T">Tipo de estado a cambiar</typeparam>
        public void ChangeState<T>() where T : State
        {
            Type nextStateType = typeof(T);

            if (!states.TryGetValue(nextStateType, out State nextState))
            {
                Debug.LogWarning($"State {nextStateType} not found in StateMachine. Add it first before using it.");
                return;
            }

            if (currentState == nextState)
            {
                // Evita reentradas innecesarias
                return;
            }

            Type prevStateType = currentState?.GetType();

            currentState?.OnExit();
            currentState = nextState;
            currentState.OnEnter();

            //Debug.Log(currentState);

            OnStateChanged?.Invoke(prevStateType, nextStateType);
        }

        #endregion

        #region Update Loops

        public void Update() => currentState?.Update();
        public void FixedUpdate() => currentState?.FixedUpdate();

        #endregion

        #region Getters

        /// <summary>
        /// Estado actual de la máquina.
        /// </summary>
        public State CurrentState => currentState;

        /// <summary>
        /// Tipo del estado actual.
        /// </summary>
        public Type CurrentStateType => currentState?.GetType();

        /// <summary>
        /// Tipo del estado actual.
        /// </summary>
        public T GetState<T>() where T : State
        {
            Type stateType = typeof(T);

            if (!states.TryGetValue(typeof(T), out State state))
            {
                Debug.LogWarning($"State {stateType} not found in StateMachine. Add it first before using it.");
                return null;
            }

            return (T)state;
        }

        #endregion
    }
}