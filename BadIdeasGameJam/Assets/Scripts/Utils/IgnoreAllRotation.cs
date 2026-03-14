using UnityEngine;

[ExecuteAlways]
public class IgnoreAllRotation : MonoBehaviour
{
    void LateUpdate() => transform.rotation = Quaternion.identity;
}