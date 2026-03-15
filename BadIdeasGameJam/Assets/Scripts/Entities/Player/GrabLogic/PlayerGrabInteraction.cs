using UnityEngine;

namespace Game.Entities.Player
{
    public class PlayerGrabInteraction : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private Vector2 throwForce = new Vector2(2, 1);

        [Header("References")]
        [SerializeField] private Transform grabSpot;
        [SerializeField] private TriggerSensor triggerSensor;

        // Variables
        private IGrabbable currentObject;
        public bool IsHolding => currentObject != null;

        public void TryGrab()
        {
            if (IsHolding) return;

            var grabbable = triggerSensor.GetClosestComponent<IGrabbable>(grabSpot.position);

            if (grabbable == null)
                return;

            currentObject = grabbable;
            currentObject.OnGrab(grabSpot);
        }

        public void Release(Vector3 forward)
        {
            if (!IsHolding) return;

            // Solo interesa la direccion en el plano XZ
            Vector3 finalThrowForce = forward;
            finalThrowForce.y = 0;
            finalThrowForce *= throwForce.x;
            finalThrowForce.y = throwForce.y;

            currentObject.OnRelease(finalThrowForce);
            currentObject = null;
        }
    }
}
