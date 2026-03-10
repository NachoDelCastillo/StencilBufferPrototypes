using UnityEngine;

public class CubeMaskMover : MonoBehaviour
{
    [SerializeField] private Camera refCam;

    [SerializeField, Range(0, 0.975f)] private float normalizedDistance;

    private void OnValidate()
    {
        if (refCam == null) return;

        normalizedDistance = Mathf.Clamp01(normalizedDistance);

        Vector3 start = new Vector3(0, 0.5f, -20);
        Vector3 end = refCam.transform.position;       // posición local de la cámara

        transform.position = Vector3.Lerp(start, end, normalizedDistance);
    }
}
