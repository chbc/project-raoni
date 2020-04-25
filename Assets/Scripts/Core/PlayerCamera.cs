using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform referenceTransform;
    [SerializeField] private BoxCollider2D collider;
    
    public static PlayerCamera Instance { get; private set; }
    
    private Transform cameraTransform;
    private Vector3 cameraPosition;
    private bool isLimited;
    private float limitedX;

    public void LimitBack(bool canUpdate)
    {
        this.isLimited = canUpdate;
        this.limitedX = this.cameraPosition.x;
    }
    
    private void Awake()
    {
        Instance = this;
        this.isLimited = false;
    }

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
            cameraPosition.x = x;

        if (this.isLimited && (x < this.limitedX))
            cameraPosition.x = this.limitedX;
        
        this.cameraTransform.position = cameraPosition;
    }
}
