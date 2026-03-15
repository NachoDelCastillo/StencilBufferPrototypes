using UnityEngine;

public class DebugController : MonoBehaviour
{
    private WorldTransitionManager _worldTransitionManager;
    private WorldTransitionManager worldTransitionManager
    {
        get
        {
            if (_worldTransitionManager == null)
                _worldTransitionManager = FindFirstObjectByType<WorldTransitionManager>();
            return _worldTransitionManager;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //CkeckOnClickItems();
            worldTransitionManager.GetInsideBox(WorldId.blueWorld);
        }

        if (Input.GetMouseButtonDown(1))
        {
            worldTransitionManager.GetOutsideCurrentBox();
        }
    }

    private void CkeckOnClickItems()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // DEBUG del raycast
        Debug.DrawRay(ray.origin, ray.direction * 10000f, Color.red, 1f);

        int layerMask = 1 << 0; // 0 = Default
        if (Physics.Raycast(ray, out hit, 10000, layerMask))
        {
            //DebugCanvas.Instance.SetDebugText("El Raycast Pego");

            // Rayo verde porque golpeó algo
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green, 2f);

            GameObject clickedObject = hit.collider.gameObject;

            if (clickedObject.TryGetComponent(out BoxWorld boxWorld))
            {
                worldTransitionManager.GetInsideBox(boxWorld);
            }
        }
        else
        {
            // Rayo verde porque golpeó algo
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, 2f);
        }
    }
}
