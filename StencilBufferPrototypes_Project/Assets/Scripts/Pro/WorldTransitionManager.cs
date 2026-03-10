using UnityEngine;

[ExecuteAlways]
public class WorldTransitionManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField, Range(0, 1f)] private float transitionLerp;
    [SerializeField] private AnimationCurve worldZoomCurve;

    [Header("References")]
    [SerializeField] WorldData fromWorld;
    [SerializeField] WorldData toWorld;

    private void Update()
    {
        InitWorlds();
        ApplyTransitionLerp();
    }

    private void InitWorlds()
    {

    }

    private void ApplyTransitionLerp()
    {
        transitionLerp = Mathf.Clamp01(transitionLerp);

        // Zoom de la camara antigua
        float startZoom = 1f;
        float endZoom = .03f;
        // Los dos pivotes estan justo en medio del World Cube, les setteamos exactamente la misma escala
        fromWorld.WorldZoom.localScale = Mathf.Lerp(startZoom, endZoom, transitionLerp) * Vector3.one;
        toWorld.CubeMaskZoom.localScale = Mathf.Lerp(startZoom, endZoom, transitionLerp) * Vector3.one;


        float tinyWorldEffectStartZoom = 5f;
        float tinyWorldEffectEndZoom = 1f;

        float t = worldZoomCurve.Evaluate(transitionLerp);
        float scale = Mathf.Lerp(tinyWorldEffectStartZoom, tinyWorldEffectEndZoom, t);

        toWorld.WorldZoom.localScale = Vector3.one * scale;
    }
}
