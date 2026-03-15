using Core.Utils;
using DG.Tweening;
using Game.Entities.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum WorldId
{
    blueWorld,
    redWorld,
    greenWorld
}
public class WorldTransitionManager : MonoBehaviour
{
    #region Inspector

    [Header("Parameters")]
    [SerializeField, Range(0, 1f)] private float transitionLerp;
    [SerializeField] private AnimationCurve transitionCurve;
    [SerializeField] private AnimationCurve worldZoomCurve;
    [SerializeField] private LayerMask innerWorldLayer;

    [SerializeField] private Vector3 minOffsetFromCamera;

    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private SerializedDictionary<WorldId, WorldData> allWorldsData;
    [SerializeField] private SerializedDictionary<WorldId, BoxWorld> allBoxWorlds;

    [Header("Debug")]

    [SerializeField] private WorldId debugOutterWorldId;
    [SerializeField] private WorldId debugInnerWorldId;

    [SerializeField] private bool ajustShit;
    [SerializeField, Range(0, 1f)] private float adjustShitSlider;

    #endregion

    #region Variables

    private int innerWorldLayerIndex = -1;

    WorldId currentWorldId;

    private bool transitionInProgress = false;

    private WorldData auxWorldData;

    #endregion

    #region Initialize

    private void Awake() => Initialize();

    private void Initialize()
    {
        // Se inicializa un mundo concreto
        SetWorld(WorldId.blueWorld);
    }

    #endregion

    #region Transition Setup

    public void GetInsideBox(WorldId worldId)
    {
        if (!CheckValidTransition()) return;
        StartCoroutine(GetInsideBox_Coroutine(allBoxWorlds[worldId]));
    }

    public void GetInsideBox(BoxWorld boxWorld)
    {
        if (!CheckValidTransition()) return;
        StartCoroutine(GetInsideBox_Coroutine(boxWorld));
    }
    IEnumerator GetInsideBox_Coroutine(BoxWorld boxWorld)
    {
        yield return SetCameraToTarget(boxWorld.transform);
        yield return SwapToInnerWorld(boxWorld.WorldId);
    }

    public void GetOutsideCurrentBox()
    {
        if (!CheckValidTransition()) return;

        // Obtener referencia a la caja en la que esta contenida el mundo en el que se esta actualmente
        // Teletransportarse desde el mundo actual, al mundo en el que esta contenida la caja
        StartCoroutine(SwapToOutsideWorld(allBoxWorlds[currentWorldId].InsideWorldId));
    }

    private bool CheckValidTransition()
    {
        bool initialValue = transitionInProgress;
        transitionInProgress = true;

        return !initialValue;
    }

    private IEnumerator SetCameraToTarget(Transform target, float duration = 2)
    {
        WorldData currentWorldData = allWorldsData[currentWorldId];
        currentWorldData.CameraTarget.DOMove(target.position, duration).SetEase(Ease.InOutCubic);

        yield return new WaitForSeconds(duration);
    }

    IEnumerator SwapToOutsideWorld(WorldId worldId)
    { yield return InitTransition(worldId, currentWorldId, true); }

    IEnumerator SwapToInnerWorld(WorldId worldId)
    { yield return InitTransition(currentWorldId, worldId, false); }

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
                baseData.cameraStack.Add(playerCamera);

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

    private IEnumerator InitTransition(WorldId outerWorldId, WorldId innerWorldId, bool reverse = false)
    {
        WorldData outerWorld = allWorldsData[outerWorldId];
        WorldData innerWorld = allWorldsData[innerWorldId];

        bool enterAuxShit = outerWorldId == innerWorldId;

        //string debugText = enterAuxShit ? "enterAuxShit TRUE" : "enterAuxShit FALSE";
        //DebugCanvas.Instance.SetDebugText(debugText, 2);

        if (enterAuxShit)
        {
            auxWorldData = Instantiate(outerWorld, new Vector3(0, 0, 600), Quaternion.identity);

            if (reverse)
                innerWorld = auxWorldData;
            else
                outerWorld = auxWorldData;
        }

        StartCoroutine(SetUpPlayerForTransition(outerWorld, innerWorld, reverse));

        // Aplicar primer frame
        ApplyTransitionLerp(reverse ? 1 : 0, outerWorld, innerWorld);

        // Fijar la camara en el enterSpot del nuevo mundo
        if (!reverse)
            innerWorld.CameraTarget.position = innerWorld.EnterSpot.position;
        // Fijar la camara en la caja del mundo actual en el nuevo mundo
        else
            outerWorld.CameraTarget.position = allBoxWorlds[innerWorldId].transform.position;


        outerWorld.Cam.enabled = true;
        innerWorld.Cam.enabled = true;

        // Obtener los datos adicionales de URP
        var baseData = outerWorld.Cam.GetUniversalAdditionalCameraData();
        var overlayData = innerWorld.Cam.GetUniversalAdditionalCameraData();

        // Configurar tipos
        baseData.renderType = CameraRenderType.Base;
        overlayData.renderType = CameraRenderType.Overlay;

        // Añadir la camara overlay al stack de la camara base
        baseData.cameraStack.Clear();
        baseData.cameraStack.Add(innerWorld.Cam);
        baseData.cameraStack.Add(playerCamera);

        // Settear los layers
        LayerUtils.SetLayerRecursively(outerWorld.Enviroment, 0); // Default layer
        LayerUtils.SetLayerRecursively(innerWorld.Enviroment, GetInnerWorldLayer()); // layer afectada por el Stencil Buffer

        if (Application.isPlaying)
            yield return PerformTransition(outerWorld, innerWorld, reverse);
    }

    private IEnumerator SetUpPlayerForTransition(WorldData outerWorld, WorldData innerWorld, bool reverse)
    {
        player.EnterTeleport();

        // Teletransportar al player al enter spot del mundo al que estas entrando
        if (!reverse)
        {
            Transform refCam = outerWorld.Cam.transform;
            if (outerWorld.WorldId == innerWorld.WorldId)
                refCam = innerWorld.Cam.transform;

            Vector3 offset = player.transform.position - refCam.position;
            player.transform.position = playerCamera.transform.position + offset;

            Vector3 landingSpotInFakeWorld = playerCamera.transform.position + minOffsetFromCamera; // + worldData.EnterSpot.localPosition;

            float duration = 2.5f;
            player.transform.DOMove(landingSpotInFakeWorld, duration).SetEase(Ease.InOutCubic);
            //player.transform.DORotate(player.transform.rotation.eulerAngles + Vector3.up * 360 * 2, duration, RotateMode.FastBeyond360);
            //player.transform.DORotate(Vector3.up * 360 * 3, duration, RotateMode.FastBeyond360);

            DebugCanvas.Instance.SetDebugText(landingSpotInFakeWorld.ToString(), 3);

            yield return new WaitForSeconds(3.6f);

            player.transform.DOKill(true);
            player.transform.position = innerWorld.EnterSpot.position;
        }

        // Teletransportar al player a encima de la caja del mundo del que has salido
        else
        {
            Transform refCam = innerWorld.Cam.transform;
            if (outerWorld.WorldId == innerWorld.WorldId)
                refCam = outerWorld.Cam.transform;

            Vector3 offset = player.transform.position - refCam.position;
            player.transform.position = playerCamera.transform.position + offset;

            Vector3 landingSpotInFakeWorld = playerCamera.transform.position + minOffsetFromCamera + Vector3.up * 1;

            float duration = 3.5f;
            player.transform.DOMove(landingSpotInFakeWorld, duration).SetEase(Ease.InOutCubic);
            //player.transform.DORotate(player.transform.rotation.eulerAngles + Vector3.up * 360 * 2, duration, RotateMode.FastBeyond360);
            //player.transform.DORotate(Vector3.up * 360 * 3, duration, RotateMode.FastBeyond360);

            DebugCanvas.Instance.SetDebugText(landingSpotInFakeWorld.ToString(), 3);

            yield return new WaitForSeconds(4.6f);

            player.transform.DOKill(true);
            player.transform.position = allBoxWorlds[innerWorld.WorldId].transform.position + Vector3.up * 1;
        }

        player.ExitTeleport();
    }

    #endregion

    #region Transition Process

    private IEnumerator PerformTransition(WorldData outerWorld, WorldData innerWorld, bool reverse = false)
    {
        //DebugCanvas.Instance.SetDebugText(innerWorld.WorldId.ToString());

        // Hacer transparentes los world cubes
        //yield return ModifyTransparentCubes(outerWorldId, innerWorldId, true, reverse);
        StartCoroutine(ModifyTransparentCubes(outerWorld, innerWorld, true, reverse));

        yield return WrapWorlds(outerWorld, innerWorld, reverse);

        yield return ModifyTransparentCubes(outerWorld, innerWorld, false, reverse);

        SetWorld(reverse ? outerWorld.WorldId : innerWorld.WorldId);

        // Terminamos transicion
        transitionInProgress = false;

        // Si se ha hecho uso de un mundo auxiliar, destruirlo
        if (auxWorldData != null)
        {
            Destroy(auxWorldData.gameObject);
            auxWorldData = null;
        }
    }

    private IEnumerator ModifyTransparentCubes(WorldData outerWorld, WorldData innerWorld, bool beforeTransition, bool reverse = false, float duration = 1.5f)
    {
        if (reverse)
        {
            if (beforeTransition)
            {
                innerWorld.CubeMaskCoverAnimator.SetVisualActive(true);
                innerWorld.CubeMaskCoverAnimator.SetOpenLerp(1);
            }
            else
            {
                yield return innerWorld.CubeMaskCoverAnimator.Close(duration);
                innerWorld.CubeMaskCoverAnimator.SetVisualActive(false);

                outerWorld.CubeMaskCoverAnimator.SetVisualActive(false);
            }
        }
        else
        {
            if (beforeTransition)
            {
                innerWorld.CubeMaskCoverAnimator.SetVisualActive(true);

                yield return innerWorld.CubeMaskCoverAnimator.Open(duration);
            }
            else
            {
                innerWorld.CubeMaskCoverAnimator.SetVisualActive(false);
            }
        }
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

    #endregion

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

    private void OnValidate()
    {
        #region Debug
        if (ajustShit && !Application.isPlaying && debugOutterWorldId != debugInnerWorldId)
        {
            InitTransition(debugOutterWorldId, debugInnerWorldId);
            ApplyTransitionLerp(adjustShitSlider, debugOutterWorldId, debugInnerWorldId);

            // Volver transparentes todos los CubeWorlds por debug
            foreach (var pair in allWorldsData)
                SetAlphaToMaterial(pair.Value.CubeWorldMaterial, 0);
        }
        #endregion

        // Settear los WorldId de cada worldData, de esta forma solo los setteo una unica vez pero los worldData saben que id representan
        foreach (var pair in allWorldsData)
            pair.Value.SetWorldId(pair.Key);
    }

#endif
}



//IEnumerator DebugDelay()
//{
//    yield return new WaitForSeconds(.5f);

//    yield return SwapToOutsideWorld(WorldId.redWorld);

//    yield return new WaitForSeconds(.5f);

//    yield return SwapToInnerWorld(WorldId.blueWorld);

//    while (true)
//    {
//        yield return new WaitForSeconds(.5f);

//        yield return SwapToInnerWorld(WorldId.redWorld);

//        yield return new WaitForSeconds(.5f);

//        yield return SwapToInnerWorld(WorldId.blueWorld);
//    }
//}