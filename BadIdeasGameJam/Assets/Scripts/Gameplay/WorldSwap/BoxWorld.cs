using UnityEngine;

public class BoxWorld : MonoBehaviour
{
    [SerializeField] private WorldId worldId;

    [SerializeField, HideInInspector] private WorldTransitionManager worldTransitionManager;
    public WorldTransitionManager WorldTransitionManager => worldTransitionManager;
    public void SetWorldTransitionManager(WorldTransitionManager worldTransitionManager) => this.worldTransitionManager = worldTransitionManager;

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
    public WorldId SetInsideWorldId(WorldId insideWorldId) => this.insideWorldId = insideWorldId;
}
