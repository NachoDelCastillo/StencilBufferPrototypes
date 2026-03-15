using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableItem : MonoBehaviour, IGrabbable
{
    private Rigidbody rb;

    #region Initialize

    private void Awake() => Initialize();
    private void Initialize()
    {
        rb = GetComponent<Rigidbody>();
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
        transform.SetParent(null);
        rb.isKinematic = false;
        rb.AddForce(releaseForce, ForceMode.Impulse);
    }
}
