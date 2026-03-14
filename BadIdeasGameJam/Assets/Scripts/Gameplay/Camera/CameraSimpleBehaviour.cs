using UnityEngine;
using Game.Entities.Player;

namespace Game.Gameplay
{
    public class CameraSimpleBehaviour : MonoBehaviour
    {
        [SerializeField] private bool followPlayer;
        [SerializeField] private Camera cam;

        [SerializeField] Transform target;

        [SerializeField] private float followSpeed = 2;

        public Transform GetCurrentCameraTransform() => cam.transform;

        private void FixedUpdate()
        {
            if (followPlayer)
                transform.position = Vector3.Lerp(transform.position, target.position, followSpeed * Time.fixedDeltaTime);
        }
    }
}
