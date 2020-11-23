using UnityEngine;
public class EnemyBehaviour : MonoBehaviour
{
    [Header("General")]
    public float walkSpeed = 2.0f;

    public bool showDebugInfo = true;

    [Header("Collision-based Reorientation")]
    public float wallCheckDistance = 1.5f;
    private bool _hitCheckOnce;

    [Header("Sphere-based Reorientation")] 
    [Tooltip("This sets the distance for how close the enemy must be to the destination point in order to create a new one")]
    public float minDistToDestPoint = 0.4f;
    [SerializeField]
    private float walkableRadius;
    [SerializeField]
    private Vector3 destinationPoint;
    private Vector3 _initalPosition;

    private void Start()
    {
        _hitCheckOnce = false;
        
        walkableRadius = 0;
        destinationPoint = transform.position;
        _initalPosition = transform.position;
        FindClosestCollider();
    }
    
    private void Update()
    {
        transform.Translate( walkSpeed * Time.deltaTime * Vector3.forward);
        CollisionReorientation();
        //SphereReorientation();
        if(showDebugInfo) DrawDebug();
    }

    private void CollisionReorientation()
    {
        if (!_hitCheckOnce && Physics.Raycast(transform.position, transform.forward, out var raycastHit, wallCheckDistance))
        {
            _hitCheckOnce = true;
            Mesh hitObjectMesh = raycastHit.collider.GetComponent<MeshFilter>().mesh;
            var vertexIndex = hitObjectMesh.triangles[raycastHit.triangleIndex * 3];
            Vector3 hitObjectNormal = hitObjectMesh.normals[vertexIndex];
            float deltaAngle = 0;
            do
            {
                //Todo: only consider the x portion of the vectors
                //Todo: readjust the random angle
                deltaAngle = Vector3.Angle(transform.forward, hitObjectNormal) + Random.Range(-90,90);
            } while (Physics.Raycast(transform.position,  transform.rotation * Quaternion.Euler(0,deltaAngle,0) * transform.forward, wallCheckDistance));
            transform.rotation *= Quaternion.Euler(0, deltaAngle, 0);
            _hitCheckOnce = false;
        }
    }

    private void SphereReorientation()
    {
        if ((destinationPoint - transform.position).magnitude < minDistToDestPoint + 1)
        {
            SetRandomDestination();
            Vector3 diffVector = destinationPoint - transform.position;
            Debug.Log(diffVector);
            //transform.rotation *= Quaternion.Euler(0,Vector3.Angle(transform.forward, diffVector),0);
            transform.LookAt(destinationPoint);
            
        }
    }

    private void FindClosestCollider()
    {
        float currentSphereRadius = 0;
        float stepSize = 0.05f;
        int colliderAmount = 0;
        Collider[] collidersOverlapped = new Collider[1];
        while (colliderAmount == 0)
        {
            currentSphereRadius += stepSize;
            colliderAmount = Physics.OverlapSphereNonAlloc(transform.position, currentSphereRadius, collidersOverlapped, LayerMask.GetMask("Room"));
        }

        walkableRadius = currentSphereRadius;
    }

    private void SetRandomDestination()
    {
        Vector3 randomXYPosition = Random.insideUnitSphere * walkableRadius;
        destinationPoint = _initalPosition + new Vector3(randomXYPosition.x , 0, randomXYPosition.z);
    }

    private void DrawDebug()
    {
        Debug.DrawRay(transform.position, transform.forward * wallCheckDistance, Color.magenta);
    }
}
