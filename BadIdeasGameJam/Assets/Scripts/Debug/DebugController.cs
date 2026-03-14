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
            CkeckOnClickItems();
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

        if (Physics.Raycast(ray, out hit))
        {
            GameObject clickedObject = hit.collider.gameObject;

            if (clickedObject.TryGetComponent(out BoxWorld boxWorld))
            {
                Debug.Log(boxWorld.gameObject.name);
                worldTransitionManager.GetInsideBox(boxWorld);
            }
        }
    }
}
