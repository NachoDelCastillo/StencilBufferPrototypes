using System;
using System.Collections;
using UnityEngine;

namespace Core.Animation
{
    /// <summary>
    /// Controla las animaciones del personaje.
    /// Wrapper con funcionalidad basica para el componente Animator
    /// </summary>
    public class AnimatorWrapper : MonoBehaviour
    {
        #region Variables

        [Header("Components")]
        [SerializeField] protected Animator anim;

        /// <summary>
        /// Evento lanzado al finalizar una animación. 
        /// </summary>
        public event Action<string> OnAnimationFinished;

        string playingAnimationName;

        #endregion

        #region MonoBehaviour Methods

        private void Awake()
        {
            if (anim == null)
                Debug.LogError($"[{nameof(AnimatorWrapper)}] Animator no asignado en {gameObject.name}");
        }

        #endregion

        #region Animation Logic

        /// <summary>
        /// Funcionalidad para ejecutar una animacion segun su nombre pasandolo a hash
        /// </summary>
        /// <param name="animName"> Nombre de la animacion </param>
        public void PlayAnimation(string animName)
        {
            playingAnimationName = animName;
            PlayAnimation(Animator.StringToHash(animName));
        }

        private void PlayAnimation(int animHash)
        {
            if (anim == null) return;
            anim.Play(animHash);
        }

        public void PlayAnimationFade(string animName, float transitionDuration = .25f)
        {
            playingAnimationName = animName;
            PlayAnimationFade(Animator.StringToHash(animName), transitionDuration);
        }

        private void PlayAnimationFade(int animHash, float transitionDuration = .1f)
        {
            if (anim == null) return;
            anim.CrossFade(animHash, transitionDuration);
        }

        private Coroutine frozenRoutine;
        public void PlayFrozenAnimationFade(string animName, float transitionDuration = 1f)
        {
            if (frozenRoutine != null)
                StopCoroutine(frozenRoutine);

            int animHash = Animator.StringToHash(animName);

            frozenRoutine = StartCoroutine(WeightedAnimationFadeRoutine(animHash, transitionDuration));
        }

        private IEnumerator WeightedAnimationFadeRoutine(int animHash, float duration)
        {
            // Seguridad mínima
            if (duration <= 0f)
                duration = 0.01f;

            // Coloca la animación destino en la layer 1, en frame fijo
            anim.Play(animHash, 1);
            anim.Update(0f); // fuerza evaluación inmediata

            // Congela TODO el Animator
            anim.speed = 0f;

            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float weight = Mathf.Clamp01(time / duration);

                float easedWeight = EaseInOutQuint(weight);

                anim.SetLayerWeight(1, easedWeight);
                yield return null;
            }

            // Asegura estado final limpio
            anim.SetLayerWeight(1, 0f);

            anim.Play(animHash, 0);
        }

        private float EaseInOutQuint(float t)
        {
            return t < 0.5f
                ? 16f * t * t * t * t * t
                : 1f - Mathf.Pow(-2f * t + 2f, 5f) / 2f;
        }

        #endregion

        #region Animation Events

        public void OnAnimFinished()
        {
            // Si se esta haciendo un Fade entre animaciones, la animacion antigua trigeara tambien el evento de "OnAnimFinished", haciendo como si fuese la actual
            // Comprobando si el hash actual del animator (animacion antigua en caso de un fade) y el hash logico de la animacion actual (animacion nueva en caso de fade) es el mismo
            // Sabemos que solo se invokara el final de la animacion si se termina la animacion logica unicamente. Aunque hayan terminado otras secundarias en el Animator.

            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            int activeAnimationInAnimator = stateInfo.shortNameHash;
            int activeAnimationInLogic = Animator.StringToHash(playingAnimationName);

            if (activeAnimationInAnimator == activeAnimationInLogic)
                OnAnimationFinished?.Invoke(playingAnimationName);
        }

        #endregion

        #region Animator Logic

        public void SetSpeed(float speed)
        {
            anim.speed = speed;
        }

        public void SetFloat(string id, float value)
        {
            anim.SetFloat(id, value);
        }

        #endregion
    }
}