using UnityEngine;

public class BoxWorld : MonoBehaviour
{
    [SerializeField] private WorldId worldId;

    // En que mundo esta metida esta caja
    private WorldId insideWorldId;

    private void Awake() => Initialize();

    private void Initialize()
    {
        WorldData insideWorldData = GetComponentInParent<WorldData>();
        insideWorldId = insideWorldData.WorldId;
    }

    public WorldId WorldId => worldId;
    public WorldId InsideWorldId => insideWorldId;
}
