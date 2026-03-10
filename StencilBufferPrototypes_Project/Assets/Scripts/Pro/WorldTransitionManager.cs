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

        // Cuadrar el mismo zoom en la camara antigua y en la nueva
        float startZoom = 1f;
        float endZoom = .03f;
        // Los dos pivotes estan justo en medio del World Cube, les setteamos exactamente la misma escala
        fromWorld.WorldZoom.localScale = Mathf.Lerp(startZoom, endZoom, transitionLerp) * Vector3.one;
        toWorld.CubeMaskZoom.localScale = Mathf.Lerp(startZoom, endZoom, transitionLerp) * Vector3.one;


        // Camara alejada del mundo al que se esta entrando al inicio y poco a poco se acerca hasta ajustaste a la camara de juego (worldZoom = 1)
        float startWorldZoom = 5f;
        float endWorldZoom = 1f;

        // Pasar el lerp por una curva
        float t = worldZoomCurve.Evaluate(transitionLerp);
        float scale = Mathf.Lerp(startWorldZoom, endWorldZoom, t);

        toWorld.WorldZoom.localScale = Vector3.one * scale;
    }
}
