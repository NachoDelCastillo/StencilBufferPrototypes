using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Game.Entities.Base
{
    public class EntityGfxTransform : MonoBehaviour
    {
        #region Variables

        [SerializeField] private float angularSpeed = 5;
        [SerializeField] private float slowedAngularSpeed = 7f;
        [SerializeField] private float linearSpeed = 2;

        // Rotacion deseada en angulos euler
        private Vector3 targetRotation;

        // Rotacion deseada en angulos euler
        private Vector3 targetPosition;

        bool globalCoordinates = false;
        bool slowedRotation = false;

        #endregion

        #region Initialize

        public virtual void Initialize()
        {
            SetTargetDirectionXZ(transform.forward);
        }

        #endregion

        #region Setters

        public void SetTargetDirectionXZ(Vector3 targetDirection, bool slowedRotation = false)
        {
            if (targetDirection.sqrMagnitude < 0.0001f)
                return;

            this.slowedRotation = slowedRotation;

            float yaw = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
            targetRotation = new Vector3(0f, yaw, 0f);
        }

        public void SetTargetGlobalPosition(Vector3 targetPosition)
        {
            globalCoordinates = true;
            this.targetPosition = targetPosition;
        }
        public void ResetTargetPosition()
        {
            globalCoordinates = false;
            this.targetPosition = Vector3.zero;
        }
        public void JumpTo(Vector3 targetPosition) => transform.DOJump(targetPosition, .5f, 1, .6f);
        public void JumpToInitPos() => transform.DOLocalJump(Vector3.zero, .5f, 1, .6f);

        #endregion

        #region MonoBehaviour

        private void Update()
        {
            InterpolateRotation();
            //InterpolatePosition();
        }

        #endregion

        #region Interpolation

        private void InterpolateRotation()
        {
            Quaternion current = transform.rotation;
            Quaternion target = Quaternion.Euler(targetRotation);

            float selectedAngularSpeed = slowedRotation ? slowedAngularSpeed : angularSpeed;

            transform.rotation = Quaternion.Slerp(
                current,
                target,
                Time.deltaTime * selectedAngularSpeed
            );
        }

        private void InterpolatePosition()
        {
            if (globalCoordinates)
            {
                transform.position = Vector3.Lerp(
                    transform.position,
                    targetPosition,
                    Time.deltaTime * linearSpeed
                );
            }
            else
            {
                transform.localPosition = Vector3.Lerp(
                    transform.localPosition,
                    targetPosition,
                    Time.deltaTime * linearSpeed
                );
            }
        }

        #endregion

        #region Stretch & Squash

        private const float deformMin = .8f;
        private float deformMax = 1.2f;
        private float stretchSquatchDuration = .15f;

        protected void DoStretch() => StartCoroutine(DoStretch_Coroutine());
        private IEnumerator DoStretch_Coroutine()
        {
            transform.DOKill(true);
            transform.DOScale(new Vector3(deformMin, deformMax, deformMin), stretchSquatchDuration);
            yield return new WaitForSeconds(stretchSquatchDuration);
            transform.DOScale(Vector3.one, stretchSquatchDuration);
        }

        protected void DoSquash() => StartCoroutine(DoSquash_Coroutine());
        private IEnumerator DoSquash_Coroutine()
        {
            transform.DOKill(true);
            transform.DOScale(new Vector3(deformMax, deformMin, deformMax), stretchSquatchDuration);
            yield return new WaitForSeconds(stretchSquatchDuration);
            transform.DOScale(Vector3.one, stretchSquatchDuration + .2f);
        }


        #endregion
    }
}
