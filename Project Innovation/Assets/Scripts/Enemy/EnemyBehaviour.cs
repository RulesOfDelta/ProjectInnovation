using System.Security.AccessControl;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("General")] public float walkSpeed = 2.0f;

    private float currentWalkSpeed;
    public bool showDebugInfo = true;
    private Transform ownTransform;
    private EnemyAttack enemyAttack;
    private GameObject player;
    public bool SawPlayer { get; private set; }
    public float sightDistance = 2.0f, sightRange = 70.0f;

    [Header("Collision-based Reorientation")]
    public bool enableCBR = true;

    public float wallCheckDistance = 1.5f;
    [Range(0, 89)] public float angleRange = 0.0f;
    [SerializeField] private LayerMask layerMask;

    [Header("Sphere-based Reorientation")]
    [Tooltip(
        "This sets the distance for how close the enemy must be to the destination point in order to create a new one")]
    public bool enableSBR = false;

    public float minDistToDestPoint = 0.4f;
    public float maxSphereSize = 100.0f;

    [SerializeField] private float walkableRadius;
    [SerializeField] private Vector3 destinationPoint;
    private Vector3 _initalPosition;
    private bool _withinDestinationRange;

    private Rigidbody rigidbody;
    
    private void Start()
    {
        currentWalkSpeed = walkSpeed;
        player = GameObject.FindWithTag("Player");

        ownTransform = GetComponent<Transform>();

        // layerMask = LayerMask.GetMask("Room");

        if (enableCBR && enableSBR) Debug.LogError("You must either enable CBR or SBR. Both are not allowed!");

        if (enableSBR)
        {
            walkableRadius = 0;
            destinationPoint = ownTransform.position;
            _initalPosition = ownTransform.position;
            FindClosestCollider();
        }

        _withinDestinationRange = false;

        enemyAttack = GetComponent<EnemyAttack>();
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //Deprecated: ownTransform.Translate(currentWalkSpeed * Time.deltaTime * transform.forward, Space.World);
        rigidbody.MovePosition(transform.position + currentWalkSpeed * Time.deltaTime * transform.forward);

        if (!SawPlayer) LookForPlayer();
        else ChasePlayer();
        
        if (!SawPlayer)
        {
            if (enableCBR)
            {
                CollisionReorientation();
            }
            else if (enableSBR)
            {
                SphereReorientation();
            }
        }

        if (showDebugInfo) DrawDebug();
    }

    private void CollisionReorientation()
    {
        if (!Physics.Raycast(ownTransform.position, ownTransform.forward, out var raycastHit, wallCheckDistance,
            layerMask)) return;

        Mesh hitObjectMesh = raycastHit.collider.GetComponent<MeshFilter>().mesh;
        var vertexIndex = hitObjectMesh.triangles[raycastHit.triangleIndex * 3];
        Vector3 hitObjectNormal =
            raycastHit.collider.gameObject.transform.TransformVector(hitObjectMesh.normals[vertexIndex]);

        Vector3 transformForwardXZ = new Vector3(transform.forward.x, 0, transform.forward.z);
        Vector3 hitObjectNormalXZ = new Vector3(hitObjectNormal.x, 0, hitObjectNormal.z);

        float leftAngleRange = -angleRange, rightAngleRange = angleRange;

        //Considers xz-plane only
        float deltaAngle = Vector3.SignedAngle(transformForwardXZ, hitObjectNormalXZ, Vector3.up) +
                           Random.Range(leftAngleRange, rightAngleRange);

        //This loop will only run if the newly generated angle intersects with another collider
        while (Physics.Raycast(transform.position, Quaternion.Euler(0, deltaAngle, 0) * transform.forward,
            wallCheckDistance * 2, layerMask))
        {
            Vector3 newDirectionVector = Quaternion.Euler(0, deltaAngle, 0) * transformForwardXZ;
            Vector3 perpendicularVector = Vector3.Cross(hitObjectNormalXZ, newDirectionVector);
            
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

            deltaAngle = Vector3.SignedAngle(transformForwardXZ, hitObjectNormalXZ, Vector3.up) +
                         Random.Range(leftAngleRange, rightAngleRange);
        }

        transform.rotation *= Quaternion.Euler(0, deltaAngle, 0);
    }

    private void SphereReorientation()
    {
        if (!_withinDestinationRange && (destinationPoint - ownTransform.position).magnitude < minDistToDestPoint)
        {
            _withinDestinationRange = true;
            SetRandomDestination();
            //Vector from current position to new destination position
            Vector3 differenceVector = destinationPoint - ownTransform.position;
            float deltaAngle = Vector3.SignedAngle(ownTransform.forward, differenceVector, Vector3.up);
            ownTransform.rotation *= Quaternion.Euler(0, deltaAngle, 0);
        }
        else
        {
            _withinDestinationRange = false;
        }
    }

    private void FindClosestCollider()
    {
        float currentSphereRadius = 0;
        float stepSize = 0.05f;
        int colliderAmount = 0;
        Collider[] collidersOverlapped = new Collider[1];
        while (colliderAmount == 0 && currentSphereRadius <= maxSphereSize)
        {
            currentSphereRadius += stepSize;
            colliderAmount = Physics.OverlapSphereNonAlloc(ownTransform.position, currentSphereRadius,
                collidersOverlapped, layerMask);
        }

        if (colliderAmount == 0)
        {
            walkableRadius = 2.0f;
            Debug.LogWarning("No collider with the correct tag was found! 'walkableRadius' was set to 2!");
        }

        else
        {
            if (showDebugInfo) Debug.Log("First collider hit was " + collidersOverlapped[0].transform.name);
            walkableRadius = currentSphereRadius - stepSize;   
        }
    }

    private void SetRandomDestination()
    {
        Vector2 randomXZPosition = Random.insideUnitCircle * walkableRadius;
        destinationPoint = _initalPosition +
                           new Vector3(randomXZPosition.x, ownTransform.lossyScale.y / 2, randomXZPosition.y);
    }

    private void DrawDebug()
    {
        Debug.DrawRay(ownTransform.position, ownTransform.forward * wallCheckDistance, Color.magenta);
        Debug.DrawLine(ownTransform.position, destinationPoint, Color.green);
    }

    private void ChasePlayer()
    {
        Vector3 differenceVector = player.transform.position - transform.position;

        if (Vector3.Distance(player.transform.position, transform.position) < 2.5f) currentWalkSpeed = 0;
        else currentWalkSpeed = walkSpeed;
        
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 
            Quaternion.LookRotation(differenceVector).eulerAngles.y, transform.rotation.eulerAngles.z);
    }

    private void LookForPlayer()
    {
        if (Vector3.Distance(player.transform.position, transform.position) <= sightDistance)
        {
            Vector3 differenceVector = player.transform.position - transform.position;
            float deltaAngle = Vector3.Angle(differenceVector, transform.forward);
            if (deltaAngle <= sightRange)
            {
                SawPlayer = true;
                Debug.Log("Saw player");
                StartCoroutine(enemyAttack.AttackLoop());
            }
        }
    }
}