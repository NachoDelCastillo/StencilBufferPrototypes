using Core.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

enum WorldId
{
    blueWorld,
    redWorld,
    greenWorld
}
//[ExecuteAlways]
public class WorldTransitionManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField, Range(0, 1f)] private float transitionLerp;
    [SerializeField] private AnimationCurve transitionCurve;
    [SerializeField] private AnimationCurve worldZoomCurve;
    [SerializeField] private LayerMask innerWorldLayer;
    [SerializeField] private SerializedDictionary<WorldId, WorldData> allWorldsData;

    [Header("Debug")]
    [SerializeField] private WorldId debugOutterWorldId;
    [SerializeField] private WorldId debugInnerWorldId;

    [SerializeField] private bool ajustShit;
    [SerializeField, Range(0, 1f)] private float adjustShitSlider;

    // Variables
    private int innerWorldLayerIndex = -1;

    WorldId currentWorldId;

    private void Awake() => Initialize();

    private void Initialize()
    {
        // Se inicializa un mundo concreto
        SetWorld(WorldId.blueWorld);

        StartCoroutine(DebugDelay());
    }

    IEnumerator DebugDelay()
    {
        yield return new WaitForSeconds(2f);

        //InitTransition(allWorldsData[WorldId.blueWorld], allWorldsData[WorldId.redWorld]);
        InitTransition(WorldId.redWorld, currentWorldId, true);
    }

    private void SetWorld(WorldId worldId)
    {
        currentWorldId = worldId;

        foreach (var pair in allWorldsData)
        {
            WorldData worldData = pair.Value;

            if (pair.Key == worldId)
            {
                var baseData = worldData.Cam.GetUniversalAdditionalCameraData();
                baseData.renderType = CameraRenderType.Base;
                baseData.cameraStack.Clear();

                float startZoom = 1f;
                float endZoom = .03f;
                worldData.WorldZoom.localScale = Mathf.Lerp(startZoom, endZoom, transitionLerp) * Vector3.one;
                worldData.CubeMaskZoom.localScale = endZoom * Vector3.one;

                SetAlphaToMaterial(worldData.CubeWorldMaterial, 0);
            }
            else
            {
                worldData.Cam.enabled = false;
                SetAlphaToMaterial(worldData.CubeWorldMaterial, 1);
            }

            LayerUtils.SetLayerRecursively(worldData.Enviroment, 0);
        }
    }

    private void InitTransition(WorldId outterWorldId, WorldId innerWorldId, bool reverse = false)
    {
        WorldData outterWorld = allWorldsData[outterWorldId];
        WorldData innerWorld = allWorldsData[innerWorldId];

        outterWorld.Cam.enabled = true;
        innerWorld.Cam.enabled = true;

        // Obtener los datos adicionales de URP
        var baseData = outterWorld.Cam.GetUniversalAdditionalCameraData();
        var overlayData = innerWorld.Cam.GetUniversalAdditionalCameraData();

        // Configurar tipos
        baseData.renderType = CameraRenderType.Base;
        overlayData.renderType = CameraRenderType.Overlay;

        // Añadir la camara overlay al stack de la camara base
        baseData.cameraStack.Clear();
        baseData.cameraStack.Add(innerWorld.Cam);

        // Settear los layers
        LayerUtils.SetLayerRecursively(outterWorld.Enviroment, 0); // Default layer
        LayerUtils.SetLayerRecursively(innerWorld.Enviroment, GetInnerWorldLayer()); // layer afectada por el Stencil Buffer

        //SetAlphaToMaterial(outterWorld.CubeWorldMaterial, 1);
        //SetAlphaToMaterial(innerWorld.CubeWorldMaterial, 1);

        // Aplicar primer frame
        ApplyTransitionLerp(reverse ? 1 : 0, outterWorld, innerWorld);

        if (Application.isPlaying)
            StartCoroutine(PerformTransition(outterWorldId, innerWorldId, reverse));
    }

    private IEnumerator PerformTransition(WorldId outerWorldId, WorldId innerWorldId, bool reverse = false)
    {
        WorldData outerWorld = allWorldsData[outerWorldId];
        WorldData innerWorld = allWorldsData[innerWorldId];

        // Hacer transparentes los world cubes
        yield return ModifyTransparentCubes(outerWorldId, innerWorldId, true, reverse);

        yield return WrapWorlds(outerWorld, innerWorld, reverse);

        yield return ModifyTransparentCubes(outerWorldId, innerWorldId, false, reverse);

        SetWorld(reverse ? outerWorldId : innerWorldId);
    }

    private IEnumerator ModifyTransparentCubes(WorldId outerWorldId, WorldId innerWorldId, bool beforeTransition, bool reverse = false, float duration = 1)
    {
        WorldData outerWorld = allWorldsData[outerWorldId];
        WorldData innerWorld = allWorldsData[innerWorldId];


        Material mat = null;

        if (reverse)
        {
            //mat = outerWorld.CubeWorldMaterial;

            if (beforeTransition)
                SetAlphaToMaterial(outerWorld.CubeWorldMaterial, 0);
            else
                SetAlphaToMaterial(outerWorld.CubeWorldMaterial, 0);
        }

        //if (mat != null)
        //{
        //    float elapsed = 0;
        //    while (elapsed < duration)
        //    {
        //        float lerp = 1 - (elapsed / duration);

        //        SetAlphaToMaterial(mat, lerp);

        //        elapsed += Time.deltaTime;
        yield return null;
        //    }
        //}
    }

    private IEnumerator WrapWorlds(WorldData outerWorld, WorldData innerWorld, bool reverse = false, float duration = 3)
    {
        // !Reverse : de 0 a 1
        // Reverse : de 1 a 0

        float elapsed = 0;
        while (elapsed < duration)
        {
            float lerp = reverse ? 1 - (elapsed / duration) : elapsed / duration;

            float curvedLerp = transitionCurve.Evaluate(lerp);
            ApplyTransitionLerp(curvedLerp, outerWorld, innerWorld);

            elapsed += Time.deltaTime;

            yield return null;
        }

        float finalValue = !reverse ? 1 : 0;
        // Aseguramos que la transición termine exactamente en 1
        ApplyTransitionLerp(finalValue, outerWorld, innerWorld);
    }

    private void ApplyTransitionLerp(float transitionLerp, WorldId outterWorldId, WorldId innerWorldId)
        => ApplyTransitionLerp(transitionLerp, allWorldsData[outterWorldId], allWorldsData[innerWorldId]);
    private void ApplyTransitionLerp(float transitionLerp, WorldData outterWorld, WorldData innerWorld)
    {
        transitionLerp = Mathf.Clamp01(transitionLerp);

        // Cuadrar el mismo zoom en la camara antigua y en la nueva
        float startZoom = 1f;
        float endZoom = .03f;
        // Los dos pivotes estan justo en medio del World Cube, les setteamos exactamente la misma escala
        outterWorld.WorldZoom.localScale = Mathf.Lerp(startZoom, endZoom, transitionLerp) * Vector3.one;
        innerWorld.CubeMaskZoom.localScale = Mathf.Lerp(startZoom, endZoom, transitionLerp) * Vector3.one;

        outterWorld.CubeMaskZoom.localScale = endZoom * Vector3.one;

        // Camara alejada del mundo al que se esta entrando al inicio y poco a poco se acerca hasta ajustaste a la camara de juego (worldZoom = 1)
        float startWorldZoom = 5f;
        float endWorldZoom = 1f;
        float t = worldZoomCurve.Evaluate(transitionLerp);
        float scale = Mathf.Lerp(startWorldZoom, endWorldZoom, t);
        innerWorld.WorldZoom.localScale = Vector3.one * scale;
    }

    #region Utils

    private int GetInnerWorldLayer()
    {
        if (innerWorldLayerIndex == -1)
        {
            if (!LayerUtils.TryGetSingleLayer(innerWorldLayer, out innerWorldLayerIndex))
                Debug.LogError($"The '{nameof(innerWorldLayer)}' LayerMask must contain exactly one layer.");
        }
        return innerWorldLayerIndex;
    }

    private void SetAlphaToMaterial(Material material, float alpha)
    {
        Color color = material.color;
        color.a = alpha;
        material.color = color;
    }

    #endregion

#if UNITY_EDITOR
    #region Debug

    private void OnValidate()
    {
        if (ajustShit && !Application.isPlaying && debugOutterWorldId != debugInnerWorldId)
        {
            InitTransition(debugOutterWorldId, debugInnerWorldId);
            ApplyTransitionLerp(adjustShitSlider, debugOutterWorldId, debugInnerWorldId);

            // Volver transparentes todos los CubeWorlds por debug
            foreach (var pair in allWorldsData)
                SetAlphaToMaterial(pair.Value.CubeWorldMaterial, 0);
        }
    }

    #endregion
#endif
}
