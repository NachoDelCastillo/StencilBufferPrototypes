using UnityEngine;

public class WorldData : MonoBehaviour
{
    // No settear esto a mano, se settea desde el WorldTransitionManager
    private WorldId worldId;

    [Header("References")]

    [SerializeField] private Camera cam;

    [Tooltip("Pivote desde donde sale la camara y todo lo demas")]
    [SerializeField] private Transform cameraTarget;

    [Tooltip("Referencia a la mascara cubo de la camara de este mundo")]
    [SerializeField] private GameObject cubeMask;

    [Tooltip("Cover de la mascara cubo")]
    [SerializeField] private Renderer cubeMaskCover;

    [Tooltip("Cover de la mascara cubo")]
    [SerializeField] private CubeMaskCoverAnimator cubeMaskCoverAnimator;

    [Tooltip("Controla como de cerca esta la mascara del cubo de la camara, tiene el pivote en la mascara del cubo")]
    [SerializeField] private Transform cubeMaskZoom;

    [Tooltip("Controla como de cerca esta la camara del mundo, manteniendo igual al cubo mascara, tiene el pivote en el target de la camara")]
    [SerializeField] private Transform worldZoom;

    [Tooltip("Referencia a la mascara cubo de la camara de este mundo")]
    [SerializeField] private Transform enviroment;

    [Tooltip("Material que usa el cubo")]
    [SerializeField] private Material cubeWorldMaterial;

    [Tooltip("Material que usa el cubo")]
    [SerializeField] private Material auxCubeWorldMaterial;

    [Tooltip("Lugar al que se accede cuando se entra a un mundo desde una caja")]
    [SerializeField] private Transform enterSpot;


    public WorldId WorldId => worldId;
    public void SetWorldId(WorldId worldId) => this.worldId = worldId;

    public Camera Cam => cam;
    public Transform CameraTarget => cameraTarget;
    public CubeMaskCoverAnimator CubeMaskCoverAnimator => cubeMaskCoverAnimator;
    public Transform CubeMaskZoom => cubeMaskZoom;
    public Transform WorldZoom => worldZoom;
    public Transform Enviroment => enviroment;
    public Material CubeWorldMaterial => cubeWorldMaterial;
    public Material AuxCubeWorldMaterial => auxCubeWorldMaterial;
    public Transform EnterSpot => enterSpot;


    private void Awake() => cubeMaskCoverAnimator.SetVisualActive(false);
}