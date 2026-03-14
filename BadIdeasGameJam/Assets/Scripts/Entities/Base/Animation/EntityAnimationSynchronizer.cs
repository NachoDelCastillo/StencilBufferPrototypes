using Core.Animation;
using Core.FSM;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entities.Base.Animation
{
    /// <summary>
    /// Sincroniza la <see cref="Core.FSM.StateMachine"/> de una entidad con un <see cref="AnimatorWrapper"/>.
    /// Permite crear y gestionar transiciones entre estados y animaciones de manera escalable.
    /// </summary>
    /// <typeparam name="TAnimId">
    /// Enum que define todas las animaciones disponibles para la entidad.
    /// Los nombres del enum deben coincidir con los nombres de los clips en el Animator (el enum debe declararlo la clase hija).
    /// Ejemplo: si existe un clip "Idle" en el Animator, el enum debe contener un valor <c>Idle</c>.
    /// </typeparam>
    public abstract class EntityAnimationSynchronizer<TAnimId> : MonoBehaviour
        where TAnimId : struct, Enum
    {
        #region Variables

        [SerializeField] private AnimatorWrapper animWrapper;

        /// <summary>
        /// Máquina de estados asociada a la entidad.
        /// </summary>
        protected StateMachine stateMachine;

        /// <summary>
        /// Diccionario que almacena hashes de animaciones generados a partir del enum <typeparamref name="TAnimId"/>.
        /// </summary>
        protected Dictionary<TAnimId, int> animHashes;

        /// <summary>
        /// Contiene mapeos de estados y transiciones hacia animaciones específicas.
        /// </summary>
        protected StateAnimationMappings<TAnimId> animatorStateMappings;

        #endregion

        #region Initialization

        /// <summary>
        /// Inicializa el sincronizador, registrando callbacks y generando hashes.
        /// </summary>
        /// <param name="stateMachine">Máquina de estados a sincronizar.</param>
        public virtual void Initialize(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));

            stateMachine.OnStateChanged += HandleStateChange;
            animWrapper.OnAnimationFinished += HandleAnimationFinish;

            animHashes = GenerateHashes<TAnimId>();
            animatorStateMappings = new StateAnimationMappings<TAnimId>();

            SetupMappings();
        }

        /// <summary>
        /// Método que debe implementar la clase hija para definir mapeos de animaciones específicos.
        /// </summary>
        protected abstract void SetupMappings();

        #endregion

        #region FSM Callbacks

        /// <summary>
        /// Gestiona los cambios de estado en la máquina de estados y dispara la animación correspondiente.
        /// </summary>
        /// <param name="prev">Estado anterior.</param>
        /// <param name="next">Nuevo estado.</param>
        protected virtual void HandleStateChange(Type prev, Type next)
        {
            if (next == null) throw new ArgumentNullException(nameof(next));

            // Animaciones específicas de transición entre estados
            if (animatorStateMappings.TryGetTransitionAnimation(prev, next, out TAnimId transitionAnimId))
            {
                PlayAnimation(transitionAnimId);
            }
            // Animacion especifica del nuevo estado
            else if (animatorStateMappings.TryGetAnimation(next, out TAnimId animId))
            {
                PlayAnimation(animId);
            }
        }

        #endregion

        #region Animation Methods & Callbacks

        protected virtual void HandleAnimationFinish(string animName)
        {
            // Parsear el string a TAnimId
            if (Enum.TryParse<TAnimId>(animName, out TAnimId animEnum))
            {
                // Si hay un mapeo de animación al finalizar, la disparamos
                if (animatorStateMappings.TryGetOnFinishAnimation(animEnum, out TAnimId nextAnim))
                {
                    PlayAnimation(nextAnim);
                }
            }
            else
            {
                Debug.LogWarning($"[EntityAnimationSynchronizer<{typeof(TAnimId).Name}>] Animation with name '{animName}' does not exist.");
            }
        }

        /// <summary>
        /// Reproduce la animación indicada por su enum convirtiendola primero a su equivalente hash.
        /// </summary>
        /// <param name="animEnum">Identificador de animación a reproducir.</param>
        protected void PlayAnimation(TAnimId animEnum, float transitionDuration = .25f)
        {
            //animWrapper.PlayAnimation(animHashes[animEnum]);
            animWrapper.PlayAnimationFade(animEnum.ToString(), transitionDuration);
        }

        /// <summary>
        /// Genera un diccionario de hashes a partir de cualquier enum.
        /// </summary>
        /// <typeparam name="TEnum">Tipo de enum.</typeparam>
        /// <returns>Diccionario donde la clave es el enum y el valor es el hash de animación.</returns>
        public static Dictionary<TEnum, int> GenerateHashes<TEnum>() where TEnum : Enum
        {
            var dict = new Dictionary<TEnum, int>();
            foreach (TEnum value in Enum.GetValues(typeof(TEnum)))
            {
                dict[value] = Animator.StringToHash(value.ToString());
            }
            return dict;
        }

        protected void SetFloat(string id, float value)
        {
            animWrapper.SetFloat(id, value);
        }

        #endregion
    }
}