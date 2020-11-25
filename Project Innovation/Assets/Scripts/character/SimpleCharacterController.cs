using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimpleCharacterController : MonoBehaviour
{
    private InputHandler handler;
    
    [SerializeField] private float acceleration = 5.0f;
    [SerializeField] private float maxVelocity = 10.0f;

    // [SerializeField] private float jumpHeight = 10.0f;
    [SerializeField] private float inAirMovementMultiplier = 0.4f;

    [SerializeField] private float mouseXSensitivity = 1.0f;
    [SerializeField] private float friction = 0.95f;

    [SerializeField] private Vector3 gravity = new Vector3(0, -9.81f, 0);

    private CharacterController characterController;
    private Vector3 acceleration3d, velocityXZ, velocityY;

    private void Awake()
    {
        if (!characterController)
            characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        //Components
        if (!handler)
            handler = GameObject.FindWithTag("InputHandler").GetComponent<InputHandler>();
        //Movement vectors
        acceleration3d = Vector3.zero;
        velocityXZ = Vector3.zero;
        velocityY = Vector3.zero;
    }

    private void Update()
    {
        acceleration3d = handler.Move.XZ() * acceleration;

        //Rotates character
        transform.Rotate(new Vector3(0, handler.Look.x * mouseXSensitivity, 0));

        //Apply friction when no movement-related key is being pressed
        //Info: Friction is only applied to XZ-plane
        if (!handler.Moving) velocityXZ *= friction;

        // //Jump when character on ground and space pressed
        // if (characterController.isGrounded && Input.GetKeyDown(KeyCode.Space))
        // {
        //     velocityY = new Vector3(0, jumpHeight, 0);
        // }

        //Constrains velocity if velocity > maxVelocity
        velocityXZ = Vector3.ClampMagnitude(velocityXZ, maxVelocity);

        //Semi-implicit euler integration
        //Movement on ground
        if (characterController.isGrounded) velocityXZ += acceleration3d;
        //Movement in air
        else velocityXZ += acceleration3d * inAirMovementMultiplier;

        //Apply gravity vector
        if(!characterController.isGrounded) velocityY += gravity;

        //Move character
        characterController.Move(transform.rotation * velocityXZ * Time.deltaTime);
        characterController.Move(velocityY * Time.deltaTime);
    }

    public void EnableControls(bool shouldBeEnabledPlease)
    {
        characterController.enabled = shouldBeEnabledPlease;
    }

    public void ResetController()
    {
        characterController.SimpleMove(Vector3.zero);
        
        acceleration3d = Vector3.zero;
        velocityXZ = Vector3.zero;
        velocityY = Vector3.zero;
    }
}