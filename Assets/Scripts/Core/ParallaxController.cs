using UnityEngine;

namespace ProjectRaoni
{
    public class ParallaxController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera = null;

        private Transform objectTransform;
        private Transform mainCameraTransform;
        private Vector3 initialPosition;
        private Vector3 currentPosition;
        private float zDistance;

        private void Start()
        {
            this.objectTransform = base.transform;
            this.initialPosition = this.objectTransform.position;
            this.currentPosition = this.initialPosition;
            this.mainCameraTransform = this.mainCamera.transform;
            this.zDistance = this.initialPosition.z - this.mainCameraTransform.position.z;
        }

        private void Update()
        {
            this.currentPosition.x = this.GetPosition();
            this.objectTransform.position = this.currentPosition;
        }

        /*
         * x = (n/Pz) * Px
         */
        private float GetPosition()
        {
            Vector3 cameraPosition = this.mainCameraTransform.position;
            float relativeXPosition = this.initialPosition.x - cameraPosition.x;
            return (cameraPosition.x + (relativeXPosition / this.zDistance));
        }
    }
}
