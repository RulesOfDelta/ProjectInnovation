using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("General")] public float walkSpeed = 2.0f;

    public bool showDebugInfo = true;
    private Transform _transform;

    [Header("Collision-based Reorientation")]
    public bool enableCBR = true;

    public float wallCheckDistance = 1.5f;
    [Range(0, 89)] public float angleRange = 0.0f;
    private int layerMask;

    [Header("Sphere-based Reorientation")]
    [Tooltip(
        "This sets the distance for how close the enemy must be to the destination point in order to create a new one")]
    public bool enableSBR = false;

    public float minDistToDestPoint = 0.4f;

    [SerializeField] private float walkableRadius;
    [SerializeField] private Vector3 destinationPoint;
    private Vector3 _initalPosition;

    private void Start()
    {
        _transform = GetComponent<Transform>();

        layerMask = LayerMask.GetMask("Room");

        if (enableSBR)
        {
            walkableRadius = 0;
            destinationPoint = _transform.position;
            _initalPosition = _transform.position;
            FindClosestCollider();
        }
    }

    private void Update()
    {
        if (enableCBR && enableSBR) Debug.LogError("You must either enable CBR or SBR. Both are not allowed!");
        _transform.Translate(walkSpeed * Time.deltaTime * transform.forward, Space.World);
        if (enableCBR)
        {
            CollisionReorientation();
        }
        else
        {
            //SphereReorientation();
        }

        if (showDebugInfo) DrawDebug();
    }

    private void CollisionReorientation()
    {
        if (!Physics.Raycast(_transform.position, _transform.forward, out var raycastHit, wallCheckDistance)) return;

        Mesh hitObjectMesh = raycastHit.collider.GetComponent<MeshFilter>().mesh;
        var vertexIndex = hitObjectMesh.triangles[raycastHit.triangleIndex * 3];
        Vector3 hitObjectNormal = raycastHit.collider.gameObject.transform.TransformVector(hitObjectMesh.normals[vertexIndex]);
        
        Vector3 transformForwardXZ = new Vector3(transform.forward.x, 0, transform.forward.z);
        Vector3 hitObjectNormalXZ = new Vector3(hitObjectNormal.x, 0, hitObjectNormal.z);

        float leftAngleRange = -angleRange, rightAngleRange = angleRange;

        //Considers xz-plane only
        float deltaAngle = Vector3.SignedAngle(transformForwardXZ, hitObjectNormalXZ, Vector3.up) + Random.Range(leftAngleRange, rightAngleRange);
        
        //This loop will only run if the newly generated angle intersects with another collider
        while (Physics.Raycast(transform.position, Quaternion.Euler(0, deltaAngle, 0) * transform.forward, wallCheckDistance * 2, layerMask))
        {
            Vector3 newDirectionVector = Quaternion.Euler(0, deltaAngle, 0) * transformForwardXZ;
            Vector3 perpendicularVector = Vector3.Cross(hitObjectNormalXZ, newDirectionVector);

            Debug.Log(perpendicularVector);

            if (perpendicularVector.y > 0)
            {
                if (showDebugInfo) Debug.Log("Unsuccessful direction-vector had a clockwise rotation");
                rightAngleRange = deltaAngle - 1;
            }
            else
            {
                if (showDebugInfo) Debug.Log("Unsuccessful direction-vector had a counter-clockwise rotation");
                leftAngleRange = deltaAngle + 1;
            }
        
            deltaAngle = Vector3.SignedAngle(transformForwardXZ, hitObjectNormalXZ, Vector3.up) + Random.Range(leftAngleRange, rightAngleRange);
        }

        transform.rotation *= Quaternion.Euler(0, deltaAngle, 0);
    }

    // private void SphereReorientation()
    // {
    //     if ((destinationPoint - _transform.position).magnitude < minDistToDestPoint + 1)
    //     {
    //         SetRandomDestination();
    //         Vector3 diffVector = destinationPoint - _transform.position;
    //         Debug.Log(diffVector);
    //         //_transform.rotation *= Quaternion.Euler(0,Vector3.Angle(_transform.forward, diffVector),0);
    //         _transform.LookAt(destinationPoint);
    //     }
    // }
    //
    private void FindClosestCollider()
    {
        float currentSphereRadius = 0;
        float stepSize = 0.05f;
        int colliderAmount = 0;
        Collider[] collidersOverlapped = new Collider[1];
        while (colliderAmount == 0)
        {
            currentSphereRadius += stepSize;
            colliderAmount = Physics.OverlapSphereNonAlloc(_transform.position, currentSphereRadius, collidersOverlapped, LayerMask.GetMask("Room"));
        }
    
        Debug.Log(collidersOverlapped[0].transform.name);
        walkableRadius = currentSphereRadius;
    }
    
    private void SetRandomDestination()
    {
        Vector2 randomXZPosition = Random.insideUnitCircle * walkableRadius;
        destinationPoint = _initalPosition + new Vector3(randomXZPosition.x, 0, randomXZPosition.y);
    }

    private void DrawDebug()
    {
        Debug.DrawRay(_transform.position, _transform.forward * wallCheckDistance, Color.magenta);
    }
}