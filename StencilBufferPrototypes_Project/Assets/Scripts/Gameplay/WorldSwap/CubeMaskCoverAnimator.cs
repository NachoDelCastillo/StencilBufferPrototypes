using DG.Tweening;
using UnityEngine;

public class CubeMaskCoverAnimator : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField, Range(0, 1f)] private float openLerp;

    [Header("References")]
    [SerializeField] private Transform topPivot;
    [SerializeField] private Transform leftPivot;
    [SerializeField] private Transform backLeftPivot;
    [SerializeField] private Transform rightPivot;
    [SerializeField] private Transform backRightPivot;

    public void Open(float duration = 1)
    {
        SetOpenLerp(0);

        Vector3 xAngle = Vector3.right * 90;
        Vector3 zAngle = Vector3.forward * 90;

        leftPivot.DOLocalRotate(zAngle, duration);
        rightPivot.DOLocalRotate(-xAngle, duration);

        topPivot.DOLocalRotate(zAngle, duration);
    }

    public void Close(float duration = 1)
    {
        SetOpenLerp(1);

        leftPivot.DOLocalRotate(Vector3.zero, duration);
        rightPivot.DOLocalRotate(Vector3.zero, duration);

        topPivot.DOLocalRotate(Vector3.zero, duration);
    }

    public void SetOpenLerp(float lerp)
    {
        Vector3 xAngle = Vector3.right * Mathf.Lerp(0, 90, lerp);
        Vector3 zAngle = Vector3.forward * Mathf.Lerp(0, 90, lerp);

        leftPivot.localRotation = Quaternion.Euler(zAngle);
        rightPivot.localRotation = Quaternion.Euler(-xAngle);

        topPivot.localRotation = Quaternion.Euler(zAngle);
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        Vector3 xAngle = Vector3.right * Mathf.Lerp(0, 90, openLerp);
        Vector3 zAngle = Vector3.forward * Mathf.Lerp(0, 90, openLerp);

        leftPivot.localRotation = Quaternion.Euler(zAngle);
        rightPivot.localRotation = Quaternion.Euler(-xAngle);

        topPivot.localRotation = Quaternion.Euler(zAngle);

        //backLeftPivot.localRotation = Quaternion.Euler(xAngle);
        //backRightPivot.localRotation = Quaternion.Euler(zAngle);
    }

#endif
}
