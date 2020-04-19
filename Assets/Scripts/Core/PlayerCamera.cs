using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform referenceTransform;
    [SerializeField] private BoxCollider2D collider;
    
    private Transform cameraTransform;
    private Vector3 cameraPosition;
    
    private void Start()
    {
        this.cameraTransform = base.transform;
        this.cameraPosition = this.cameraTransform.position;
    }

    private void Update()
    {
        Bounds bounds = this.collider.bounds;
        float x = referenceTransform.position.x;

        if (x < bounds.min.x)
            cameraPosition.x = bounds.min.x;
        else if (x > bounds.max.x)
            cameraPosition.x = bounds.max.x;
        else
            cameraPosition.x = referenceTransform.position.x;

        this.cameraTransform.position = cameraPosition;
    }
}
