using UnityEngine;

namespace ProjectRaoni
{
    public class ParallaxController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera = null;
        [SerializeField] private BoxCollider cameraBoundaries;
        [SerializeField] private Transform virtualCameraTransform;
        [SerializeField] private float cameraOffset;

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
            this.zDistance = this.initialPosition.z - this.virtualCameraTransform.position.z;
            this.mainCameraTransform = this.mainCamera.transform;
        }

        private void Update()
        {
            this.currentPosition.x = this.GetPosition();
            
            if (this.IsInsideCameraBoundaries())
                this.objectTransform.position = this.currentPosition;
        }

        /*
         * x = (n/Pz) * Px
         */
        private float GetPosition()
        {
            Vector3 cameraPosition = this.virtualCameraTransform.position;
            float relativeXPosition = this.initialPosition.x - cameraPosition.x;
            return (cameraPosition.x + (relativeXPosition / this.zDistance));
        }

        private bool IsInsideCameraBoundaries()
        {
            Bounds bounds = this.cameraBoundaries.bounds; 
            float cameraPosition = this.mainCameraTransform.position.x;
            
            return
            (
                (cameraPosition - this.cameraOffset > bounds.min.x) && 
                (cameraPosition + this.cameraOffset < bounds.max.x)
            );
        }
    }
}
