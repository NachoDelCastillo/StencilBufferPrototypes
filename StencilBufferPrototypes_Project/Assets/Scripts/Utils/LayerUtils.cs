using UnityEngine;

public static class LayerUtils
{
    public static bool TryGetSingleLayer(LayerMask mask, out int layer)
    {
        int value = mask.value;

        // comprobar que solo hay un bit activo
        if (value != 0 && (value & (value - 1)) == 0)
        {
            layer = Mathf.RoundToInt(Mathf.Log(value, 2));
            return true;
        }

        layer = -1;
        return false;
    }

    public static void SetLayerRecursively(Transform obj, int layerIndex)
    {
        obj.gameObject.layer = layerIndex;

        foreach (Transform child in obj)
            SetLayerRecursively(child, layerIndex);
    }
}