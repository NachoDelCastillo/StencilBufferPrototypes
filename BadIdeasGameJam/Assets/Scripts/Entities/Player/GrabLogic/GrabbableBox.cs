using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxWorld))]
public class GrabbableBox : MonoBehaviour, IGrabbable
{
    private Rigidbody rb;
    private BoxWorld boxWorld;

    #region Initialize

    private void Awake() => Initialize();
    private void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        boxWorld = GetComponent<BoxWorld>();
    }

    #endregion

    public void OnGrab(Transform grabPoint)
    {
        rb.isKinematic = true;
        transform.SetParent(grabPoint);
        transform.localPosition = Vector3.zero;
    }

    public void OnRelease(Vector3 releaseForce)
    {
        transform.SetParent(boxWorld.WorldTransitionManager.AllWorldsData[boxWorld.InsideWorldId].Enviroment);
        rb.isKinematic = false;
        rb.AddForce(releaseForce, ForceMode.Impulse);
    }
}
