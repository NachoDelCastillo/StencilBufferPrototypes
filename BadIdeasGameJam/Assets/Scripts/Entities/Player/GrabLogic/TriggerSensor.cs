using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerSensor : MonoBehaviour
{
    [SerializeField] private LayerMask detectionMask;

    private readonly HashSet<Collider> colliders = new();

    public IReadOnlyCollection<Collider> Colliders => colliders;

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsInLayerMask(other.gameObject.layer))
            return;

        colliders.Add(other);
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsInLayerMask(other.gameObject.layer))
            return;

        colliders.Remove(other);
    }

    bool IsInLayerMask(int layer)
    {
        return (detectionMask.value & (1 << layer)) != 0;
    }

    public bool TryGetComponentInRange<T>(out T result)
    {
        foreach (var col in colliders)
        {
            if (col == null) continue;

            if (col.TryGetComponent(out result))
                return true;
        }

        result = default;
        return false;
    }

    public IEnumerable<T> GetComponentsInRange<T>()
    {
        foreach (var col in colliders)
        {
            if (col == null) continue;

            if (col.TryGetComponent<T>(out var comp))
                yield return comp;
        }
    }

    public T GetClosestComponent<T>(Vector3 origin)
    {
        float bestDist = float.MaxValue;
        T best = default;

        foreach (var col in colliders)
        {
            if (col == null) continue;

            if (!col.TryGetComponent(out T comp))
                continue;

            float dist = (col.transform.position - origin).sqrMagnitude;

            if (dist < bestDist)
            {
                bestDist = dist;
                best = comp;
            }
        }

        return best;
    }
}