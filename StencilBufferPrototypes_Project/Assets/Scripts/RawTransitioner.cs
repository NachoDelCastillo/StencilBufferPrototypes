using UnityEngine;

[ExecuteAlways]
public class RawTransitioner : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField, Range(0, 1f)] private float transitionLerp;

    [Header("References")]
    [SerializeField] private Camera newCamera;
    [SerializeField] private Transform newCameraZoom;
    [SerializeField] private Camera oldCamera;
    [SerializeField] private Transform oldCameraZoom;

    [SerializeField] private Transform oldCubeMask;
    [SerializeField] private Transform newCubeMask;

    [SerializeField] private Transform tinyWorldEffectZoom;

    [SerializeField] private AnimationCurve zoomCurve;

    private void Update()
    {
        if (oldCamera == null) return;

        transitionLerp = Mathf.Clamp01(transitionLerp);

        // Movimiento de la mascara del cubo en el nuevo escenario
        //Vector3 startCubeMaskPos = new Vector3(0, 0.5f, -50);
        //Vector3 endCubeMaskPos = newCamera.transform.position;
        ////cubeMask.transform.position = Vector3.Lerp(startCubeMaskPos, endCubeMaskPos, transitionLerp);
        //newCubeMask.transform.position = Vector3.Lerp(startCubeMaskPos, endCubeMaskPos, transitionLerp);

        // Zoom de la camara antigua
        float startZoom = 1f;
        float endZoom = .03f;
        oldCameraZoom.localScale = Mathf.Lerp(startZoom, endZoom, transitionLerp) * Vector3.one;
        newCameraZoom.localScale = Mathf.Lerp(startZoom, endZoom, transitionLerp) * Vector3.one;


        float tinyWorldEffectStartZoom = 5f;
        float tinyWorldEffectEndZoom = 1f;

        float t = zoomCurve.Evaluate(transitionLerp);
        float scale = Mathf.Lerp(tinyWorldEffectStartZoom, tinyWorldEffectEndZoom, t);

        tinyWorldEffectZoom.localScale = Vector3.one * scale;

        //Shit();
    }

    //private void Shit()
    //{
    //    if (!oldCubeMask || !newCubeMask || !oldCamera || !newCamera) return;

    //    Vector3 offset = oldCamera.transform.position - oldCubeMask.position;

    //    Vector3 relativeCameraPosition = newCubeMask.position + offset;

    //    newShitOffset.transform.position = relativeCameraPosition;
    //}
}
