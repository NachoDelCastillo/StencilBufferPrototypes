using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CubeMaskCoverAnimator : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField, Range(0, 1f)] private float openLerpOnEditor;

    [Header("References")]
    [SerializeField] private GameObject visualObj;

    [SerializeField] private Transform leftPivot;
    [SerializeField] private Transform rightPivot;
    [SerializeField] private Transform backLeftPivot;
    [SerializeField] private Transform backRightPivot;

    [SerializeField] private Transform topLeftPivot;
    [SerializeField] private Transform topRightPivot;
    [SerializeField] private Transform topBackLeftPivot;
    [SerializeField] private Transform topBackRightPivot;

    [SerializeField] AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public IEnumerator Open(float duration = 1)
    {
        SetOpenLerp(0);

        float quarterDuration = duration * .25f;
        float halfDuration = duration * .5f;

        Vector3 xHalfAngle = Vector3.right * 135;
        Vector3 zHalfAngle = Vector3.forward * 135;

        topBackLeftPivot.DOLocalRotate(xHalfAngle, quarterDuration);
        topRightPivot.DOLocalRotate(-xHalfAngle, quarterDuration);

        yield return new WaitForSeconds(quarterDuration);

        topLeftPivot.DOLocalRotate(zHalfAngle, quarterDuration);
        topBackRightPivot.DOLocalRotate(-zHalfAngle, quarterDuration);

        yield return new WaitForSeconds(quarterDuration);

        Vector3 xAngle = Vector3.right * 90;
        Vector3 zAngle = Vector3.forward * 90;

        leftPivot.DOLocalRotate(zAngle, duration);
        rightPivot.DOLocalRotate(-xAngle, duration);
        backLeftPivot.DOLocalRotate(xAngle, duration);
        backRightPivot.DOLocalRotate(-zAngle, duration);

        topLeftPivot.DOLocalRotate(zAngle, halfDuration);
        topRightPivot.DOLocalRotate(-xAngle, halfDuration);
        topBackLeftPivot.DOLocalRotate(xAngle, halfDuration);
        topBackRightPivot.DOLocalRotate(-zAngle, halfDuration);

        yield return new WaitForSeconds(halfDuration);
    }

    public IEnumerator Close(float duration = 1)
    {
        SetOpenLerp(1);

        float quarterDuration = duration * .25f;
        float halfDuration = duration * .5f;

        Vector3 xAngle = Vector3.right * 135;
        Vector3 zAngle = Vector3.forward * 135;

        leftPivot.DOLocalRotate(Vector3.zero, halfDuration);
        rightPivot.DOLocalRotate(Vector3.zero, halfDuration);
        backLeftPivot.DOLocalRotate(Vector3.zero, halfDuration);
        backRightPivot.DOLocalRotate(Vector3.zero, halfDuration);

        topLeftPivot.DOLocalRotate(zAngle, halfDuration);
        topRightPivot.DOLocalRotate(-xAngle, halfDuration);
        topBackLeftPivot.DOLocalRotate(xAngle, halfDuration);
        topBackRightPivot.DOLocalRotate(-zAngle, halfDuration);

        yield return new WaitForSeconds(halfDuration);

        topBackLeftPivot.DOLocalRotate(Vector3.zero, quarterDuration);
        topRightPivot.DOLocalRotate(Vector3.zero, quarterDuration);

        yield return new WaitForSeconds(quarterDuration);

        topLeftPivot.DOLocalRotate(Vector3.zero, quarterDuration);
        topBackRightPivot.DOLocalRotate(Vector3.zero, quarterDuration);

        yield return new WaitForSeconds(quarterDuration);
    }

    public void SetOpenLerp(float openLerp)
    {
        if (openLerp < .5f)
        {
            float firstPartLerp = Mathf.InverseLerp(0, .5f, openLerp);
            firstPartLerp = easeCurve.Evaluate(firstPartLerp);

            Vector3 xHalfAngle = Vector3.right * Mathf.Lerp(0, 135, firstPartLerp);
            Vector3 zHalfAngle = Vector3.forward * Mathf.Lerp(0, 135, firstPartLerp);

            topLeftPivot.localRotation = Quaternion.Euler(zHalfAngle);
            topRightPivot.localRotation = Quaternion.Euler(-xHalfAngle);
            topBackLeftPivot.localRotation = Quaternion.Euler(xHalfAngle);
            topBackRightPivot.localRotation = Quaternion.Euler(-zHalfAngle);

            // Settear a 0
            leftPivot.localRotation = Quaternion.Euler(Vector3.zero);
            rightPivot.localRotation = Quaternion.Euler(Vector3.zero);
            backLeftPivot.localRotation = Quaternion.Euler(Vector3.zero);
            backRightPivot.localRotation = Quaternion.Euler(Vector3.zero);
        }

        else
        {
            float secondPartLerp = Mathf.InverseLerp(.5f, 1, openLerp);
            secondPartLerp = easeCurve.Evaluate(secondPartLerp);

            Vector3 xAngle = Vector3.right * Mathf.Lerp(0, 90, secondPartLerp);
            Vector3 zAngle = Vector3.forward * Mathf.Lerp(0, 90, secondPartLerp);

            leftPivot.localRotation = Quaternion.Euler(zAngle);
            rightPivot.localRotation = Quaternion.Euler(-xAngle);
            backLeftPivot.localRotation = Quaternion.Euler(xAngle);
            backRightPivot.localRotation = Quaternion.Euler(-zAngle);

            Vector3 xHalfToNormalAngle = Vector3.right * Mathf.Lerp(135, 90, secondPartLerp);
            Vector3 zHalfToNormalAngle = Vector3.forward * Mathf.Lerp(135, 90, secondPartLerp);

            topLeftPivot.localRotation = Quaternion.Euler(zHalfToNormalAngle);
            topRightPivot.localRotation = Quaternion.Euler(-xHalfToNormalAngle);
            topBackLeftPivot.localRotation = Quaternion.Euler(xHalfToNormalAngle);
            topBackRightPivot.localRotation = Quaternion.Euler(-zHalfToNormalAngle);
        }
    }

    public void SetVisualActive(bool active) => visualObj.SetActive(active);

#if UNITY_EDITOR

    private void OnValidate()
    {
        SetOpenLerp(openLerpOnEditor);
    }

#endif
}
