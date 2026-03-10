using UnityEngine;

public class WorldData : MonoBehaviour
{
    [Header("References")]

    [SerializeField] private Camera cam;

    [Tooltip("Referencia a la mascara cubo de la camara de este mundo")]
    [SerializeField] private GameObject cubeMask;

    [Tooltip("Controla como de cerca esta la mascara del cubo de la camara, tiene el pivote en la mascara del cubo")]
    [SerializeField] private Transform cubeMaskZoom;

    [Tooltip("Controla como de cerca esta la camara del mundo, manteniendo igual al cubo mascara, tiene el pivote en el target de la camara")]
    [SerializeField] private Transform worldZoom;

    public Transform CubeMaskZoom => cubeMaskZoom;
    public Transform WorldZoom => worldZoom;
}