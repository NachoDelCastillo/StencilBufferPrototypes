using UnityEngine;

namespace Game.Entities.Player
{
    /// <summary>
    /// Esta clase centraliza toda la lógica de interacción del jugador,
    /// Gestiona las interacciones del jugador con el mundo, incluyendo:
    /// 1. La mecánica de agarrar objetos.
    /// 2. Entrar y salir de cajas (<c>BoxWorld</c>).
    /// </summary>
    public class PlayerInteractions : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private Vector2 throwForce = new Vector2(2, 1);

        [Header("References")]
        [SerializeField] private Transform grabSpot;
        [SerializeField] private TriggerSensor triggerSensor;

        // Variables
        private IGrabbable currentObject;
        public bool IsHolding => currentObject != null;

        #region Grab

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

        #endregion

        #region Teleport

        public void TryTeleport()
        {
            IGrabbable grabbable = triggerSensor.GetClosestComponent<IGrabbable>(grabSpot.position);

            if (grabbable != null && (grabbable as Component).TryGetComponent<BoxWorld>(out BoxWorld boxWorld))
            {
                boxWorld.WorldTransitionManager.GetInsideBox(boxWorld);
            }
        }

        #endregion
    }
}
